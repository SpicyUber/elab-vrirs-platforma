using Application.DTOs.CourseEnrollment;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.CourseEnrollment
{
    public class EnrollUsingEmailCsvCommand : IRequest<List<UserCourseEnrollmentInfo>>
    {
        public Guid CourseId { get; set; }
        public Stream EmailCsv { get; set; }
    }
}
