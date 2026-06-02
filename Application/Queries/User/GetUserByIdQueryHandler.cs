using Application.DTOs.User;
using Infrastructure.Persistence.FileService.Implementation;
using Infrastructure.Persistence.FileService.Interfaces;
using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.User
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserProfileInfo>
    {
        private readonly IUnitOfWork uow;
        private readonly IFileService<Stream> fileService;
        private readonly UserManager<Domain.Entities.User> userManager;

        public GetUserByIdQueryHandler(IUnitOfWork uow, IFileService<Stream> fileService, UserManager<Domain.Entities.User> userManager)
        {
            this.uow = uow;
            this.fileService = fileService;
            this.userManager = userManager;
        }

        public async Task<UserProfileInfo> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user =
                await uow.UserRepository
                .Query()
                .Include(u => u.AvatarFile)
                .Where(u => u.Id == request.Id)
                .FirstAsync(cancellationToken);

            var roles = string.Join(',', (await userManager.GetRolesAsync(user)));

            var avatarInBase64 = await GetAvatarInBase64(user, cancellationToken);

            return new UserProfileInfo(user, roles, avatarInBase64);
        }

        private async Task<string> GetAvatarInBase64(Domain.Entities.User user, CancellationToken cancellationToken)
        {
            if(user?.AvatarFile?.StoragePath == null) return null;

            using Stream stream = await fileService.DownloadAsync(user.AvatarFile.StoragePath, cancellationToken);
            using MemoryStream memoryStream = new();

            await stream.CopyToAsync(memoryStream, cancellationToken);

            return Convert.ToBase64String(memoryStream.ToArray());
        }


    }
}
