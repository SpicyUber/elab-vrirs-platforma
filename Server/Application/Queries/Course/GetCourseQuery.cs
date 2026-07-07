using Application.DTOs.Course;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Course
{
    public class GetCourseQuery : IRequest<CourseInfo>
    {
        public Guid Id { get; set; }
        public GetCourseQuery(Guid id) => Id = id;
    }
}
