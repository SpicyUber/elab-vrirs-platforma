using Application.DTOs.User;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.FileService.Interfaces;
using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Commands.User
{
    public class UploadAvatarCommandHandler : IRequestHandler<UploadAvatarCommand, UserProfileInfo>
    {
        private readonly IFileService<Stream> fileService;
        private readonly UserManager<Domain.Entities.User> userManager;
        private readonly IUnitOfWork uow;
        public UploadAvatarCommandHandler(IFileService<Stream> fileService, UserManager<Domain.Entities.User> userManager, IUnitOfWork uow)
        {
            this.fileService = fileService;
            this.userManager = userManager;
            this.uow = uow;
        }

        public async Task<UserProfileInfo> Handle(UploadAvatarCommand request, CancellationToken cancellationToken)
        { 
            FileMetadata fileMetadata = new()
            {
                Name = request.Name,
                Mime = request.Mime,
                Extension = request.Extension,
                SizeInBytes = request.SizeInBytes,
                Status = FileStatus.Uploading,
            };

            uow.FileMetadataRepository.Add(fileMetadata);

            await uow.SaveChangesAsync(cancellationToken);

            try
            {

                await fileService.UploadAsync(
                    request.AvatarUploadStream,
                    fileMetadata.Id + fileMetadata.Extension,
                    cancellationToken);

                fileMetadata.StoragePath = fileMetadata.Id + fileMetadata.Extension;
                fileMetadata.Status = FileStatus.UploadSuccess;
                await uow.SaveChangesAsync(cancellationToken);
            }
            catch (Exception)
            {
                fileMetadata.Status = FileStatus.UploadFailed;
                await uow.SaveChangesAsync(cancellationToken);
                throw new InvalidOperationException("Upload Failed!");
            }

            var user = new Domain.Entities.User() { Id = request.UserId };
            string role = (await userManager.GetRolesAsync(user))[0];

            user = uow.UserRepository.GetById(user.Id);
            user.AvatarFileId = fileMetadata.Id;

            await uow.SaveChangesAsync(cancellationToken);

            if(request.AvatarUploadStream.CanSeek)
            request.AvatarUploadStream.Position = 0;

            using MemoryStream memoryStream = new();

            await request.AvatarUploadStream.CopyToAsync(memoryStream, cancellationToken);

            string avatarInBase64 = Convert.ToBase64String(memoryStream.ToArray());

            return new(user,avatarInBase64,role);
        }
    }
}
