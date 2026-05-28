using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Infrastructure.Persistence.FileService.Interfaces;
using Infrastructure.Persistence.FileService.Options;
using Microsoft.Extensions.Options;

namespace Infrastructure.Persistence.FileService.Implementation
{
    public class FileService : IFileService
    {
        private readonly IOptions<AzureBlobStorageOptions> options;

        public FileService(IOptions<AzureBlobStorageOptions> options)
        {
            this.options = options;
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
            var azureStream = await blobClient.OpenReadAsync(blobReadOptions, cancellationToken);

            var memoryStream = new MemoryStream();
            await azureStream.CopyToAsync(memoryStream);

            return memoryStream.ToArray();
        }

        private BlobClient CreateBlobClient(string path)
        {
            return new BlobClient(options.Value.ConnectionString, "vrirs", path);
        }

        public async Task UploadAsync(byte[] bytes, string path, CancellationToken cancellationToken)
        {
            var blobClient = CreateBlobClient(path);
            await blobClient.UploadAsync(new MemoryStream(bytes), cancellationToken);
        } 
    }
}
