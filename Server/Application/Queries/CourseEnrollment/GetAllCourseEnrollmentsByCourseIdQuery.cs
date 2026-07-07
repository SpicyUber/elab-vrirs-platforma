using Application.DTOs.CourseEnrollment;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.CourseEnrollment
{
    public class GetAllCourseEnrollmentsByCourseIdQuery : IRequest<List<UserCourseEnrollmentInfo>>
    {
        public Guid CourseId { get; set; }
    }
}
