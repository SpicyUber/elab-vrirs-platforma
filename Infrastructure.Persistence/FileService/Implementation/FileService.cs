using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Infrastructure.Persistence.FileService.Interfaces;
using Infrastructure.Persistence.FileService.Options;
using Microsoft.Extensions.Options;
using System.IO;

namespace Infrastructure.Persistence.FileService.Implementation
{
    public class FileService : IFileService<Stream>
    {
        private readonly AzureBlobStorageOptions options;

        private BlobClient CreateBlobClient(string path) => new(options.ConnectionString, options.BlobContainerName, path);

        public FileService(IOptions<AzureBlobStorageOptions> options)
        {
            this.options = options.Value;
        }

        public async Task DeleteAsync(string path, CancellationToken cancellationToken)
        {
            var blobClient = CreateBlobClient(path);
            await blobClient.DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: cancellationToken);
        }

        public async Task<Stream> DownloadAsync(string path, CancellationToken cancellationToken)
        {
            var blobClient = CreateBlobClient(path);
            var blobReadOptions = new BlobOpenReadOptions(allowModifications: false);

            return await blobClient.OpenReadAsync(blobReadOptions, cancellationToken);
        }

        public async Task UploadAsync(Stream content, string path, CancellationToken cancellationToken)
        {
            var blobClient = CreateBlobClient(path);
            using MemoryStream memoryStream = new();

            await content.CopyToAsync(memoryStream, cancellationToken);
            await blobClient.UploadAsync(content, cancellationToken);
        }
    }
}
