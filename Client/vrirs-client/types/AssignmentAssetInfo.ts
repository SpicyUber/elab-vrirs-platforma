import type { AssetType } from "./AssetType";
import type { UploadStatus } from "./UploadStatus";

export interface AssignmentAssetInfo {
    assignmentId: string;
    fileMetadataId: string;
    fileName: string;
    assetType: AssetType;
    uploadStatus: UploadStatus;
}