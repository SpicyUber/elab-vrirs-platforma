using Application.DTOs.Assignment;
using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Assignment
{
    public class GetAssignmentByIdQueryHandler : IRequestHandler<GetAssignmentByIdQuery, AssignmentInfo>
    {
        private readonly IUnitOfWork uow;

        public GetAssignmentByIdQueryHandler(IUnitOfWork uow) => this.uow = uow;

        public async Task<AssignmentInfo> Handle(GetAssignmentByIdQuery request, CancellationToken cancellationToken)
        {
            return new AssignmentInfo(uow.AssignmentRepository.GetById(request.AssignmentId));
        }
    }
}
