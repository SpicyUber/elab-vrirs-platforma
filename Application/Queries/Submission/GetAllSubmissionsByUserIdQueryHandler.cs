using Application.DTOs.Submission;
using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Submission
{
    public class GetAllSubmissionsByUserIdQueryHandler : IRequestHandler<GetAllSubmissionsByUserIdQuery, List<SubmissionInfo>>
    {
        private readonly IUnitOfWork uow;

        public GetAllSubmissionsByUserIdQueryHandler(IUnitOfWork uow) { this.uow = uow; }

        public async Task<List<SubmissionInfo>> Handle(GetAllSubmissionsByUserIdQuery request, CancellationToken cancellationToken)
        {
            return await uow.SubmissionRepository.Query()
                .Include(s => s.Student)
                .Include(s => s.Assignment)
                .Where(s => s.StudentUserId.Equals(request.UserId))
                .Select(s => new SubmissionInfo(s))
                .ToListAsync(cancellationToken);
        }
    }
}
