using Application.DTOs.User;
using Infrastructure.Persistence.FileService.Implementation;
using Infrastructure.Persistence.FileService.Interfaces;
using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Commands.User
{
    public class EditUserCommandHandler : IRequestHandler<EditUserCommand, UserProfileInfo>
    {
        private readonly IUnitOfWork uow;
        private readonly UserManager<Domain.Entities.User> userManager;
        private readonly IFileService<Stream> fileService;

        private static readonly Regex indexRegex =
        new(@"^\d{4}/\d{4}$", RegexOptions.Compiled);

        public EditUserCommandHandler(IUnitOfWork uow, UserManager<Domain.Entities.User> userManager, IFileService<Stream> fileService)
        {
            this.uow = uow;
            this.userManager = userManager;
            this.fileService = fileService;
        }

        public async Task<UserProfileInfo> Handle(EditUserCommand request, CancellationToken cancellationToken)
        {
            if(!Validate(request)) throw new ValidationException("Name or Index is invalid!");

            var user = uow.UserRepository.GetById(request.Id);

            if(user == null || !user.IsActive) throw new InvalidOperationException("User not found");

            user.FullName = request.FullName;
            user.IndexNumber = request.IndexNumber;

            await uow.SaveChangesAsync(cancellationToken);

            user =
                await uow.UserRepository
                .Query()
                .Include(u => u.AvatarFile)
                .Where(u => u.Id == request.Id)
                .FirstAsync(cancellationToken);

            var roles = string.Join(',',(await userManager.GetRolesAsync(user)));

            var avatarInBase64 = await GetAvatarInBase64(user, cancellationToken);

            return new UserProfileInfo(user, roles, avatarInBase64);
        }

        private bool Validate(EditUserCommand request)
        {
            List<Predicate<EditUserCommand>> validators =
                [
                    IsNameValid,
                    IsIndexValid
                ];

            return validators.All(predicate => predicate(request));
        }

        private async Task<string> GetAvatarInBase64(Domain.Entities.User user, CancellationToken cancellationToken)
        {
            if(user?.AvatarFile?.StoragePath == null) return null;

            using Stream stream = await fileService.DownloadAsync(user.AvatarFile.StoragePath, cancellationToken);
            using MemoryStream memoryStream = new();

            await stream.CopyToAsync(memoryStream, cancellationToken);

            return Convert.ToBase64String(memoryStream.ToArray());
        }

        private bool IsNameValid(EditUserCommand r) => !string.IsNullOrWhiteSpace(r.FullName);
        private bool IsIndexValid(EditUserCommand r) => r.IndexNumber == null || indexRegex.IsMatch(r.IndexNumber);
    }
}
