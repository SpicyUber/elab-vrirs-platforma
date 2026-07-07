import type { AssetType } from "./AssetType";
import type { UploadStatus } from "./UploadStatus";

export interface ProjectAssetInfo {
  submissionId: string;
  fileMetadataId: string;

  fileName: string;

  assetType: AssetType;
  uploadStatus: UploadStatus;
}