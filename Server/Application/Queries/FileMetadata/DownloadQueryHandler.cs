using Application.DTOs.FileMetadata;
using Infrastructure.Persistence.FileService.Interfaces;
using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.FileMetadata
{
    public class DownloadQueryHandler : IRequestHandler<DownloadQuery, DownloadPackage>
    {
        private readonly IUnitOfWork uow;
        private readonly IFileService<Stream> fileService;

        public DownloadQueryHandler(IUnitOfWork uow, IFileService<Stream> fileService)
        {
            this.fileService = fileService;
            this.uow = uow;
        }

        public async Task<DownloadPackage> Handle(DownloadQuery request, CancellationToken cancellationToken)
        {
            var fileMetadata = uow.FileMetadataRepository.GetById(request.FileId);
            if(fileMetadata == null) throw new InvalidOperationException("File not found.");
            var stream = await fileService.DownloadAsync(fileMetadata.StoragePath, cancellationToken);

            return new DownloadPackage()
            {
                Stream = stream,
                ContentType = fileMetadata.Mime,
                FileNameWithExtension = fileMetadata.Name + fileMetadata.Extension
            };
        }
    }
}
