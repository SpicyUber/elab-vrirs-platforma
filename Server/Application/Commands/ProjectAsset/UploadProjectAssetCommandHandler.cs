using Application.DTOs.ProjectAsset;
using Application.DTOs.Submission;
using Azure.Core;
using Domain.Entities;
using Domain.Enums;
using FileTypeChecker;
using FileTypeChecker.Abstracts;
using FileTypeChecker.Types;
using Infrastructure.Persistence.FileService.Interfaces;
using Infrastructure.Persistence.FileService.Options;
using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Threading;

namespace Application.Commands.ProjectAsset
{
    public class UploadProjectAssetCommandHandler : IRequestHandler<UploadProjectAssetCommand, ProjectAssetInfo>
    {
        private readonly IUnitOfWork uow;
        private readonly IFileService<Stream> fileService;
        private readonly UploadOptions uploadOptions;
        private readonly HashSet<string> sourceCodeExtensions = ["cs", "c", "cpp", "py", "java"];

        public UploadProjectAssetCommandHandler(IUnitOfWork uow, IFileService<Stream> fileService, IOptions<UploadOptions> uploadOptions)
        {
            this.uow = uow;
            this.fileService = fileService;
            this.uploadOptions = uploadOptions.Value;
        }

        public async Task<ProjectAssetInfo> Handle(UploadProjectAssetCommand request, CancellationToken cancellationToken)
        {
            var submission = (await uow.SubmissionRepository.Query().Where(s => s.Id == request.SubmissionId).Include(s => s.Assignment).FirstOrDefaultAsync(cancellationToken));

            if(submission == null) throw new InvalidOperationException("Submission not found!");

            if(!submission.Assignment.AllowProjectUpload) throw new InvalidOperationException("Cannot upload project, uploads are disabled on this assignment!");

            if(submission.Status != SubmissionStatus.Draft) throw new InvalidOperationException("Cannot attach assets to published submission!");

            ValidateFileUpload(request);

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

            try
            {
                await fileService.UploadAsync(
                    request.AssetUploadStream,
                    fileMetadata.Id + fileMetadata.Extension,
                    cancellationToken);

                fileMetadata.StoragePath = fileMetadata.Id + fileMetadata.Extension;
                fileMetadata.Status = FileStatus.UploadSuccess;

                await uow.SaveChangesAsync(cancellationToken);
            }
            catch(Exception)
            {
                fileMetadata.Status = FileStatus.UploadFailed;
                await uow.SaveChangesAsync(cancellationToken);
                throw new InvalidOperationException("Upload Failed!");
            }

            var asset =
            new Domain.Entities.ProjectAsset()
            {
                FileMetadataId = fileMetadata.Id,
                FileMetadata = fileMetadata,
                UploadStatus = UploadStatus.Validated,
                SubmissionId = request.SubmissionId,

                AssetType = await GetAssetType(request, cancellationToken)
            };

            uow.ProjectAssetRepository.Add(asset);
            await uow.SaveChangesAsync(cancellationToken);

            return new(asset);
        }

        private async Task<AssetType> GetAssetType(UploadProjectAssetCommand request, CancellationToken cancellationToken)
        {
            List<Func<UploadProjectAssetCommand, CancellationToken, Task<AssetType?>>>
                assetTypeDetectors =
                [
                    TryGetAssetTypeFromMagicBytes,
                    TryGetAssetTypeFromMime,
                    TryGetAssetTypeFromExtension
                ];

            foreach(var method in assetTypeDetectors)
            {
                AssetType? type = await method.Invoke(request, cancellationToken);

                if(type != null)
                    return (AssetType)type;
            }

            return AssetType.Other;
        }

        private async Task<AssetType?> TryGetAssetTypeFromExtension(UploadProjectAssetCommand request, CancellationToken token)
        {
            if(sourceCodeExtensions.Contains(request.Extension.ToLower()))
                return AssetType.SourceCode;

            return null;
        }

        private async Task<AssetType?> TryGetAssetTypeFromMime(UploadProjectAssetCommand request, CancellationToken token)
        {
            if(request.Mime.StartsWith("text"))
                return AssetType.Text;

            return null;
        }

        private async Task<AssetType?> TryGetAssetTypeFromMagicBytes(UploadProjectAssetCommand request, CancellationToken cancellationToken)
        {
            if(await FileTypeValidator.IsArchiveAsync(request.AssetUploadStream, cancellationToken))
                return AssetType.Archive;

            if(request.AssetUploadStream.CanSeek)
                request.AssetUploadStream.Position = 0;

            if(await FileTypeValidator.IsAsync<ExtensibleMarkupLanguage>(request.AssetUploadStream, cancellationToken))
                return AssetType.DiagramFile;

            if(request.AssetUploadStream.CanSeek)
                request.AssetUploadStream.Position = 0;

            return null;
        }

        public void ValidateFileUpload(UploadProjectAssetCommand request)
        {
            if(request.SizeInBytes > uploadOptions.FileSizeLimitInBytes)
                throw new InvalidOperationException($"Asset must be under {uploadOptions.FileSizeLimitInBytes / 1024} KB.");
            if(request.Name.Length == 0)
                throw new InvalidOperationException("File name cannot be empty.");
        }
    }
}
