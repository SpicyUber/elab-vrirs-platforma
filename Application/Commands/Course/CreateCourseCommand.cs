using Application.DTOs.Course;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Course
{
    public class CreateCourseCommand : IRequest<CourseInfo>
    {
        public string Name { get; set; } = string.Empty;


        public string? Description { get; set; }

        public Guid CreatedByUserId { get; set; } 
    }
}
