using Application.DTOs.CourseEnrollment;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Course
{
    public class GetCourseEnrollmentRoleByCourseAndUserIdQuery : IRequest<EnrollmentRole>
    {
        public Guid CourseId { get; set; }
        public Guid UserId { get; set; }
    }
}
