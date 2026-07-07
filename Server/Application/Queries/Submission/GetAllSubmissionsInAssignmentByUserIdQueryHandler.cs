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
    public class GetAllSubmissionsInAssignmentByUserIdQueryHandler : IRequestHandler<GetAllSubmissionsInAssignmentByUserIdQuery, List<SubmissionInfo>>
    {
        private readonly IUnitOfWork uow;

        public GetAllSubmissionsInAssignmentByUserIdQueryHandler(IUnitOfWork uow) { this.uow = uow; }

        public async Task<List<SubmissionInfo>> Handle(GetAllSubmissionsInAssignmentByUserIdQuery request, CancellationToken cancellationToken)
        {
            return await uow.SubmissionRepository.Query()
                .Include(s => s.Student)
                .Include(s => s.Assignment)
                .Where(s => s.StudentUserId.Equals(request.UserId) && s.AssignmentId.Equals(request.AssignmentId))
                .Select(s => new SubmissionInfo(s))
                .ToListAsync(cancellationToken);
        }
    }
}
