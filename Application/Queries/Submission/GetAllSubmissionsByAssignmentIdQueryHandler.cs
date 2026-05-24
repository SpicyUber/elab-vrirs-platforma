using Application.DTOs.Submission;
using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Submission
{
    public class GetAllSubmissionsByAssignmentIdQueryHandler : IRequestHandler<GetAllSubmissionsByAssignmentIdQuery, List<SubmissionInfo>>
    {
        private readonly IUnitOfWork uow;

        public GetAllSubmissionsByAssignmentIdQueryHandler(IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public async Task<List<SubmissionInfo>> Handle(GetAllSubmissionsByAssignmentIdQuery request, CancellationToken cancellationToken)
        {
            return uow.SubmissionRepository.Query().Where(s => s.AssignmentId == request.AssignmentId).Select(s => new SubmissionInfo(s)).ToList();
        }
    }
}
