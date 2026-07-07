using Application.Commands.ProjectAsset;
using Application.DTOs.AssignmentAsset;
using Application.DTOs.ProjectAsset;
using Domain.Entities;
using Domain.Enums;
using FileTypeChecker;
using FileTypeChecker.Types;
using Infrastructure.Persistence.FileService.Interfaces;
using Infrastructure.Persistence.FileService.Options;
using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.AssignmentAsset
{
    public class UploadAssignmentAssetCommandHandler : IRequestHandler<UploadAssignmentAssetCommand, AssignmentAssetInfo>
    {
        private readonly IUnitOfWork uow;
        private readonly IFileService<Stream> fileService;
        private readonly UploadOptions uploadOptions;
        private readonly HashSet<string> sourceCodeExtensions = ["cs", "c", "cpp", "py", "java"];

        public UploadAssignmentAssetCommandHandler(IUnitOfWork uow, IFileService<Stream> fileService, IOptions<UploadOptions> uploadOptions)
        {
            this.uow = uow;
            this.fileService = fileService;
            this.uploadOptions = uploadOptions.Value;
        }

        public async Task<AssignmentAssetInfo> Handle(UploadAssignmentAssetCommand request, CancellationToken cancellationToken)
        {
            var assignment = await uow.AssignmentRepository.Query().Where(a => a.Id == request.AssignmentId).FirstOrDefaultAsync(cancellationToken);
            if(assignment == null) throw new InvalidOperationException("Assignment not found!");

            if(assignment.Location != AssignmentLocation.Class) throw new InvalidOperationException("Cannot upload asset on non-class assignment!");

            Guid courseId = uow.AssignmentRepository.GetById(request.AssignmentId).CourseId;

            bool isTeacher = await uow.CourseEnrollmentRepository.Query()
                                                                 .Where(ce => ce.UserId == request.UserId
                                                                 && ce.CourseId == courseId
                                                                 && ce.EnrollmentRole == EnrollmentRole.Teacher)
                                                                 .AnyAsync(cancellationToken);

            if(!isTeacher) throw new InvalidOperationException("Must be a teacher on this course!");

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
            new Domain.Entities.AssignmentAsset()
            {
                FileMetadataId = fileMetadata.Id,
                FileMetadata = fileMetadata,

                AssignmentId = request.AssignmentId,

                AssetType = await GetAssetType(request, cancellationToken)
            };

            uow.AssignmentAssetRepository.Add(asset);
            await uow.SaveChangesAsync(cancellationToken);

            return new(asset);
        }

        private async Task<AssetType> GetAssetType(UploadAssignmentAssetCommand request, CancellationToken cancellationToken)
        {
            List<Func<UploadAssignmentAssetCommand, CancellationToken, Task<AssetType?>>>
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

        private async Task<AssetType?> TryGetAssetTypeFromExtension(UploadAssignmentAssetCommand request, CancellationToken token)
        {
            if(sourceCodeExtensions.Contains(request.Extension.ToLower()))
                return AssetType.SourceCode;

            return null;
        }

        private async Task<AssetType?> TryGetAssetTypeFromMime(UploadAssignmentAssetCommand request, CancellationToken token)
        {
            if(request.Mime.StartsWith("text"))
                return AssetType.Text;

            return null;
        }

        private async Task<AssetType?> TryGetAssetTypeFromMagicBytes(UploadAssignmentAssetCommand request, CancellationToken cancellationToken)
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

        public void ValidateFileUpload(UploadAssignmentAssetCommand request)
        {
            if(request.SizeInBytes > uploadOptions.FileSizeLimitInBytes)
                throw new InvalidOperationException($"Asset must be under {uploadOptions.FileSizeLimitInBytes / 1024} KB.");
            if(request.Name.Length == 0)
                throw new InvalidOperationException("File name cannot be empty.");
        }
    }
}

