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
                await uow.SubmissionReviewRepository
                .Query()
                .Include(sr => sr.ReviewedByUser)
                .Where(sr => sr.SubmissionId == request.Id)
                .Select(sr => new SubmissionReviewInfo(sr))
                .ToListAsync(cancellationToken);

            return result;
        }
    }
}
