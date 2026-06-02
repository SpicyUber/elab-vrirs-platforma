using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.AssignmentAsset
{
    public class AssignmentAssetInfo
    {
        public Guid AssignmentId { get; set; }

        public Guid FileMetadataId { get; set; }
        public string FileName { get; set; }

        public AssetType AssetType { get; set; } = AssetType.Other;
        public UploadStatus UploadStatus { get; set; } = UploadStatus.UploadStarted;

        public AssignmentAssetInfo(Domain.Entities.AssignmentAsset assignmentAsset)
        {
            AssignmentId = assignmentAsset.AssignmentId;
            FileMetadataId = assignmentAsset.FileMetadataId;

            FileName = assignmentAsset.FileMetadata?.Name ?? "Unknown";

            AssetType = assignmentAsset.AssetType;
            UploadStatus = assignmentAsset.UploadStatus;
        }
    }
}
