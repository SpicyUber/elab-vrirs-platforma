using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Infrastructure.Persistence.FileService.Interfaces;
using Infrastructure.Persistence.FileService.Options;
using Microsoft.Extensions.Options;
using System.IO;

namespace Infrastructure.Persistence.FileService.Implementation
{
    public class FileService : IFileService
    {
        private readonly AzureBlobStorageOptions options;

        private BlobClient CreateBlobClient(string path) => new(options.ConnectionString, "vrirs", path);

        public FileService(IOptions<AzureBlobStorageOptions> options)
        {
            this.options = options.Value;
        }

        public async Task DeleteAsync(string path, CancellationToken cancellationToken)
        {
            var blobClient = CreateBlobClient(path);
            var azureResponse = await blobClient.DeleteAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: cancellationToken);
        }

        public async Task<byte[]> DownloadAsync(string path, CancellationToken cancellationToken)
        {
            var blobClient = CreateBlobClient(path);
            var blobReadOptions = new BlobOpenReadOptions(allowModifications: false);

            using Stream azureStream = await blobClient.OpenReadAsync(blobReadOptions, cancellationToken);
            using MemoryStream memoryStream = new();

            await azureStream.CopyToAsync(memoryStream, cancellationToken);
            return memoryStream.ToArray();

        }

        public async Task UploadAsync(byte[] bytes, string path, CancellationToken cancellationToken)
        {
            var blobClient = CreateBlobClient(path);

            using MemoryStream memoryStream = new(bytes);
            await blobClient.UploadAsync(memoryStream, cancellationToken);
        }
    }
}
