using Application.DTOs.Assignment;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Assignment
{
    public class GetAllAssignmentsByCourseIdQuery : IRequest<List<AssignmentInfo>>
    {
        public Guid CourseId { get; set; }  
    }
}
