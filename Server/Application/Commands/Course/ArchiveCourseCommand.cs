using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Course
{
    public class ArchiveCourseCommand : IRequest
    {
        public Guid CourseId { get; set; }
    }
}
