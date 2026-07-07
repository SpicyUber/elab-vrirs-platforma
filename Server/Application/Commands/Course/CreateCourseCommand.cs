using Application.DTOs.Course;
using Domain.Enums;
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

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public CourseCategory Category { get; set; }

        public Guid CreatedByUserId { get; set; }
    }
}
