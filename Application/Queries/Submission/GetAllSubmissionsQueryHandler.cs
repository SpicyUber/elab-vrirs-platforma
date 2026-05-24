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
    public class GetAllSubmissionsQueryHandler : IRequestHandler<GetAllSubmissionsQuery,List<SubmissionInfo>>
    {
        private readonly IUnitOfWork uow;

        public GetAllSubmissionsQueryHandler(IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public async Task<List<SubmissionInfo>> Handle(GetAllSubmissionsQuery request, CancellationToken cancellationToken)
        {
            return uow.SubmissionRepository.GetAll().Select(s => new SubmissionInfo(s)).ToList();
        }
    }
}
