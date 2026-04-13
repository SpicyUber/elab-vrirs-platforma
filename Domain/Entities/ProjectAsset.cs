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
        public Guid Id { get; set; }
        public Guid SubmissionId { get; set; }
        public AssetType AssetType { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public UploadStatus UploadStatus { get; set; } = UploadStatus.Uploaded;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }

        public Submission Submission { get; set; } = null!;
    }
}
