using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ProjectAsset
    {
        public Guid SubmissionId { get; set; }

        public Guid FileMetadataId { get; set; }
        public AssetType AssetType { get; set; } = AssetType.Other;

        public UploadStatus UploadStatus { get; set; } = UploadStatus.UploadStarted;

        public FileMetadata FileMetadata {get; set;} = null!;
        public Submission Submission { get; set; } = null!;
    }
}
