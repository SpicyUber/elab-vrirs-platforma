using Application.DTOs.SubmissionReview;
using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.SubmissionReview
{
    public class GetAllReviewsBySubmissionIdQueryHandler : IRequestHandler<GetAllReviewsBySubmissionIdQuery, List<SubmissionReviewInfo>>
    {
        private readonly IUnitOfWork uow;

        public GetAllReviewsBySubmissionIdQueryHandler(IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public async Task<List<SubmissionReviewInfo>> Handle(GetAllReviewsBySubmissionIdQuery request, CancellationToken cancellationToken)
        {
            var result =
                (await uow.SubmissionRepository
                .Query()
                .Where(s => s.Id == request.SubmissionId)
                .Include(s => s.Reviews)
                .ThenInclude(r => r.ReviewedByUser)
                .FirstAsync(cancellationToken))
                .Reviews
                .Select(r => new SubmissionReviewInfo(r))
                .OrderBy(r => r.ReviewedAt)
                .ToList();
                
            return result;
        }
    }
}
