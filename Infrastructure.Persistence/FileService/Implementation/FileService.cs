using Azure.Storage.Blobs;
using Infrastructure.Persistence.FileService.Interfaces;
using Infrastructure.Persistence.FileService.Options;
using Microsoft.Extensions.Options;

namespace Infrastructure.Persistence.FileService.Implementation
{
    public class FileService : IFileService
    {
        private readonly BlobClient blobClient;

        public FileService(IOptions<AzureBlobStorageOptions> options) 
        {
            blobClient = new(new Uri(options.Value.ConnectionString));
        }

        public Task DeleteAsync(string path, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DownloadAsync(string path, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UploadAsync(Stream stream, string path, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
