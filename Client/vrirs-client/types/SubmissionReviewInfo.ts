export interface SubmissionReviewInfo {
  reviewId: string;
  submissionId: string;
  reviewedByUserId: string | null;
  reviewStatus: string;
  reviewComment: string | null;
  reviewedAt: string;
  points: number;
  submissionTitle: string;
  reviewedByUserFullName: string;
}