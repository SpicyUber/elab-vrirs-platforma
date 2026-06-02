using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.FileMetadata
{
    public class DownloadPackage
    {
        public Stream Stream { get; set; }
        public string ContentType { get; set; }
        public string FileNameWithExtension { get; set; }
    }
}
