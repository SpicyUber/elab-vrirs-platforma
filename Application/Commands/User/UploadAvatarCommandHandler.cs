using Application.DTOs.User;
using Azure.Core;
using Domain.Entities;
using Domain.Enums;
using FileTypeChecker;
using Infrastructure.Persistence.FileService.Interfaces;
using Infrastructure.Persistence.FileService.Options;
using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Threading;

namespace Application.Commands.User
{
    public class UploadAvatarCommandHandler : IRequestHandler<UploadAvatarCommand, string>
    {
        private readonly IFileService<Stream> fileService;
        private readonly UserManager<Domain.Entities.User> userManager;
        private readonly IUnitOfWork uow;
        private readonly UploadOptions uploadOptions;

        public UploadAvatarCommandHandler(IFileService<Stream> fileService, UserManager<Domain.Entities.User> userManager, IUnitOfWork uow, IOptions<UploadOptions> uploadOptions)
        {
            this.fileService = fileService;
            this.userManager = userManager;
            this.uow = uow;
            this.uploadOptions = uploadOptions.Value;
        }

        public async Task<string> Handle(UploadAvatarCommand request, CancellationToken cancellationToken)
        {
            FileMetadata fileMetadata = await CreateNewFile(request, cancellationToken);

            await TryAvatarUpload(request, fileMetadata, cancellationToken);
            await AttachAvatarToUser(request, fileMetadata, cancellationToken);

            string avatarInBase64 = await ConvertStreamToAvatarBase64(request, cancellationToken);

            return avatarInBase64;
        }

        private async Task<string> ConvertStreamToAvatarBase64(UploadAvatarCommand request, CancellationToken cancellationToken)
        {
            using MemoryStream memoryStream = new();
            await request.AvatarUploadStream.CopyToAsync(memoryStream, cancellationToken);

            return Convert.ToBase64String(memoryStream.ToArray());
        }

        private async Task AttachAvatarToUser(UploadAvatarCommand request, FileMetadata fileMetadata, CancellationToken cancellationToken)
        {
            var user = new Domain.Entities.User() { Id = request.UserId };

            user = uow.UserRepository.GetById(user.Id);
            user.AvatarFileId = fileMetadata.Id;

            await uow.SaveChangesAsync(cancellationToken);
        }

        private async Task TryAvatarUpload(UploadAvatarCommand request, FileMetadata fileMetadata, CancellationToken cancellationToken)
        {
            try
            {
                await ValidateAvatarUpload(request, cancellationToken);
                await fileService.UploadAsync(request.AvatarUploadStream, fileMetadata.Id + fileMetadata.Extension, cancellationToken);

                fileMetadata.StoragePath = fileMetadata.Id + fileMetadata.Extension;
                fileMetadata.Status = FileStatus.UploadSuccess;

                if(request.AvatarUploadStream.CanSeek)
                    request.AvatarUploadStream.Position = 0;

                await uow.SaveChangesAsync(cancellationToken);
            }
            catch(Exception)
            {
                fileMetadata.Status = FileStatus.UploadFailed;
                await uow.SaveChangesAsync(cancellationToken);

                if(request.AvatarUploadStream.CanSeek)
                    request.AvatarUploadStream.Position = 0;

                throw new InvalidOperationException("Upload Failed!");
            }

        }

        private async Task<FileMetadata> CreateNewFile(UploadAvatarCommand request, CancellationToken cancellationToken)
        {
            FileMetadata fileMetadata = new()
            {
                Name = request.Name,
                Mime = request.Mime,
                Extension = request.Extension,
                SizeInBytes = request.SizeInBytes,
                Status = FileStatus.Uploading,
                UploadedByUserId = request.UserId
            };

            uow.FileMetadataRepository.Add(fileMetadata);

            await uow.SaveChangesAsync(cancellationToken);
            return fileMetadata;
        }

        private async Task ValidateAvatarUpload(UploadAvatarCommand request, CancellationToken cancellationToken)
        {

            if(!await FileTypeValidator.IsImageAsync(request.AvatarUploadStream, cancellationToken))
                throw new InvalidOperationException("Avatar must be in jpeg or png format.");

            if(request.SizeInBytes > uploadOptions.AvatarSizeLimitInBytes)
                throw new InvalidOperationException($"Avatar must be under {uploadOptions.AvatarSizeLimitInBytes / 1024} KB.");

            if(request.Name.Length == 0)
                throw new InvalidOperationException("File name cannot be empty.");

            if(request.AvatarUploadStream.CanSeek)
                request.AvatarUploadStream.Position = 0;
        }

    }
}
