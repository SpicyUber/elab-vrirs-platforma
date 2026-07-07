using Application.DTOs.Assignment;
using MediatR;

namespace Application.Queries.Assignment
{
    public class GetAllAssignmentsByUserIdQuery : IRequest<List<AssignmentInfo>>
    {
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
    }
}
