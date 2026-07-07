using Application.DTOs.SubmissionReview;
using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.SubmissionReview
{
    public class GetSubmissionReviewByUserIdQueryHandler : IRequestHandler<GetSubmissionReviewByUserIdQuery, List<SubmissionReviewInfo>>
    {
        private readonly IUnitOfWork uow;

        public GetSubmissionReviewByUserIdQueryHandler(IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public async Task<List<SubmissionReviewInfo>> Handle(GetSubmissionReviewByUserIdQuery request, CancellationToken cancellationToken)
        {
            return await uow.SubmissionReviewRepository
                            .Query()
                            .Include(sr => sr.Submission)
                            .Where(sr => sr.Submission.StudentUserId == request.UserId)
                            .OrderBy(sr => sr.ReviewedAt)
                            .Select(sr => new SubmissionReviewInfo(sr))
                            .ToListAsync(cancellationToken);
        }
    }
}
