using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.SubmissionReview
{
    public class SubmissionReviewInfo
    {
        public Guid ReviewId { get; set; }
        public Guid SubmissionId { get; set; }

        public Guid? ReviewedByUserId { get; set; } = null;
        public ReviewStatus ReviewStatus { get; set; }

        public string? ReviewComment { get; set; }
        public DateTime ReviewedAt { get; set; }

        public int Points { get; set; }

        public string SubmissionTitle { get; set; }
        public string ReviewedByUserFullName { get; set; }

        public SubmissionReviewInfo(Domain.Entities.SubmissionReview submissionReview)
        {
            ReviewedAt = submissionReview.ReviewedAt;

            ReviewId = submissionReview.Id;
            SubmissionId = submissionReview.SubmissionId;

            ReviewStatus = submissionReview.ReviewStatus;

            ReviewComment = submissionReview.ReviewComment;

            Points = submissionReview.Points;

            SubmissionTitle = submissionReview.Submission?.Title ?? "Unknown";
            ReviewedByUserFullName = submissionReview.ReviewedByUser?.FullName ?? "System";
        }
    }
}
