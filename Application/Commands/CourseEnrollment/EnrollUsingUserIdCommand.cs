using Application.DTOs.CourseEnrollment;
using MediatR;

namespace Application.Commands.CourseEnrollment
{
    public class EnrollUsingUserIdCommand : IRequest<UserCourseEnrollmentInfo>
    {
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
        public bool IsTeacher { get; set; }
    }
}
