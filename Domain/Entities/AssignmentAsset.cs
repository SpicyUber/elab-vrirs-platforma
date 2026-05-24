using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class AssignmentAsset
    {
        public Guid AssignmentId { get; set; }

        public Guid FileMetadataId { get; set; }
        public AssetType AssetType { get; set; } = AssetType.Other;
        public UploadStatus UploadStatus { get; set; } = UploadStatus.UploadStarted;

        public Assignment Assignment { get; set; } = null!;
        public FileMetadata FileMetadata { get; set; } = null!;
    }
}
