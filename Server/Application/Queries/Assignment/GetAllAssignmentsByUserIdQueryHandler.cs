using Application.DTOs.Assignment;
using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Assignment
{
    public class GetAllAssignmentsByUserIdQueryHandler : IRequestHandler<GetAllAssignmentsByUserIdQuery, List<AssignmentInfo>>
    {
        private readonly IUnitOfWork uow;

        public GetAllAssignmentsByUserIdQueryHandler(IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public async Task<List<AssignmentInfo>> Handle(GetAllAssignmentsByUserIdQuery request, CancellationToken cancellationToken)
        {
            return await uow.AssignmentRepository.Query()
                .Where(a => a.CreatedByUserId == request.UserId)
                .Include(a => a.Course)
                .Include(a => a.CreatedByUser)
                .Select(a => new AssignmentInfo(a))
                .ToListAsync(cancellationToken);
        }
    }
}
