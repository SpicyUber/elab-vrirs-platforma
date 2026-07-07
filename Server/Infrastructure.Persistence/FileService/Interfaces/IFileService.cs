using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.FileService.Interfaces
{
    public interface IFileService<T>
    {
        public Task UploadAsync(T content,string path,CancellationToken cancellationToken);

        public Task DeleteAsync(string path, CancellationToken cancellationToken);

        public Task<T> DownloadAsync(string path, CancellationToken cancellationToken);
    }
}
