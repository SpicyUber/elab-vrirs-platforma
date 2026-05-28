using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.FileService.Options
{
    public class AzureBlobStorageOptions
    {
        public string ConnectionString { get; set; }
        public string BlobContainerName { get; set; }
    }
}
