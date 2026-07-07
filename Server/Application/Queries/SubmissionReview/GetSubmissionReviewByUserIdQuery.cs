using Application.DTOs.SubmissionReview;
using MediatR;

namespace Application.Queries.SubmissionReview
{
    public class GetSubmissionReviewByUserIdQuery : IRequest<List<SubmissionReviewInfo>>
    {
        public Guid UserId { get; set; }
    }
}
