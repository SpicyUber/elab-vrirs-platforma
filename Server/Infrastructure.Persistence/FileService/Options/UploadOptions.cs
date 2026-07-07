using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.FileService.Options
{
    public class UploadOptions
    {
        public long AvatarSizeLimitInBytes { get; set; }
        public long FileSizeLimitInBytes { get; set; }
    }
}
