using Application.DTOs.Assignment;
using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.Assignment
{
    public class GetAllAssignmentsByCourseIdQueryHandler : IRequestHandler<GetAllAssignmentsByCourseIdQuery, List<AssignmentInfo>>
    {
        private readonly IUnitOfWork uow;

        public GetAllAssignmentsByCourseIdQueryHandler(IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public async Task<List<AssignmentInfo>> Handle(GetAllAssignmentsByCourseIdQuery request, CancellationToken cancellationToken)
        {
            return await uow.AssignmentRepository.Query()
                .Where(a => a.CourseId == request.CourseId)
                .Include(a => a.Course)
                .Include(a => a.CreatedByUser)
                .Select(a => new AssignmentInfo(a))
                .ToListAsync(cancellationToken);
        }
    }
}
