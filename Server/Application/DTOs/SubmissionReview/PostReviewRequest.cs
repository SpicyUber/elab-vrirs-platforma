using Domain.Enums;

namespace Application.DTOs.SubmissionReview
{
    public class PostReviewRequest
    {
        public ReviewStatus ReviewStatus { get; set; }

        public string? ReviewComment { get; set; }

        public int Points { get; set; }
    }
}
