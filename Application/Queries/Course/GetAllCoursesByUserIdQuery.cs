using Application.DTOs.Course;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Course
{
    public class GetAllCoursesByUserIdQuery : IRequest<List<CourseInfo>>
    {
        public Guid UserId { get; set; }
    }
}
