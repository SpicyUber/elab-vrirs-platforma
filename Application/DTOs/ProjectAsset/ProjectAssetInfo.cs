using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ProjectAsset
{
    public class ProjectAssetInfo
    {
        public ProjectAssetInfo(Domain.Entities.ProjectAsset projectAsset)
        {
            SubmissionId = projectAsset.SubmissionId;
            FileMetadataId = projectAsset.FileMetadataId;

            FileName = projectAsset.FileMetadata?.Name ?? "Unknown";

            AssetType = projectAsset.AssetType;
            UploadStatus = projectAsset.UploadStatus;
        }

        public Guid SubmissionId { get; set; }

        public Guid FileMetadataId { get; set; }
        public string FileName { get; set; }

        public AssetType AssetType { get; set; } = AssetType.Other;

        public UploadStatus UploadStatus { get; set; } = UploadStatus.UploadStarted;
    }
}
