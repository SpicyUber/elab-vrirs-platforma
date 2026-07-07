using Application.DTOs.Submission;
using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.Submission
{
    public class GetSubmissionByIdQueryHandler : IRequestHandler<GetSubmissionByIdQuery, SubmissionInfo>
    {
        private readonly IUnitOfWork uow;
        public GetSubmissionByIdQueryHandler(IUnitOfWork uow) => this.uow = uow;

        public async Task<SubmissionInfo> Handle(GetSubmissionByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return await uow.SubmissionRepository.Query()
                    .Include(s => s.Assignment)
                    .Include(s => s.Student)
                    .Where(s => s.Id == request.SubmissionId)
                    .Select(s => new SubmissionInfo(s))
                    .FirstAsync(cancellationToken);
            }
            catch(Exception e)
            {
                throw new InvalidOperationException("Submission not found.");
            }
        }
    }
}
