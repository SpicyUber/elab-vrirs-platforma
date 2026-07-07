using Infrastructure.Persistence.FileService.Interfaces;
using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.AssignmentAsset
{
    public class DeleteAssignmentAssetByIdCommandHandler : IRequestHandler<DeleteAssignmentAssetByIdCommand>
    {
        public DeleteAssignmentAssetByIdCommandHandler(IUnitOfWork uow, IFileService<Stream> fileService)
        {
            this.uow = uow;
            this.fileService = fileService;
        }
        

        private readonly IUnitOfWork uow;
        private readonly IFileService<Stream> fileService;

        public async Task Handle(DeleteAssignmentAssetByIdCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var asset = await uow.AssignmentAssetRepository
                    .Query()
                    .Include(a => a.FileMetadata)
                    .Where
                    ( 
                    (a) => a.AssignmentId == request.AssignmentId 
                    && a.FileMetadataId == request.FileMetadataId
                    && a.FileMetadata.Status == Domain.Enums.FileStatus.UploadSuccess
                    )
                    .FirstAsync(cancellationToken);
                
                uow.AssignmentAssetRepository.Delete(asset);
                await fileService.DeleteAsync(asset.FileMetadata.StoragePath,cancellationToken);
                
                uow.FileMetadataRepository.Delete(asset.FileMetadata);

                await uow.SaveChangesAsync(cancellationToken);
            }
            catch(Exception ex) 
            {
                throw new InvalidOperationException(ex.Message);
            }
            return;
        }
    }
}
