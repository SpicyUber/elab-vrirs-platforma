using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.FileService.Interfaces
{
    public interface IFileService
    {
        public Task UploadAsync(byte[] bytes,string path,CancellationToken cancellationToken);

        public Task DeleteAsync(string path, CancellationToken cancellationToken);

        public Task<byte[]> DownloadAsync(string path, CancellationToken cancellationToken);
    }
}
