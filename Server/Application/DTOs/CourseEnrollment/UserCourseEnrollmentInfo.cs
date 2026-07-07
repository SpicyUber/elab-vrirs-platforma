using Domain.Entities;
using Domain.Enums;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.CourseEnrollment
{
    public class UserCourseEnrollmentInfo
    {
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }

        public string UserAvatarInBase64 { get; set; }

        public string UserFullName { get; set; }
        public string CourseName { get; set; }

        public string UserEmail { get; set; }

        public EnrollmentStatus EnrollmentStatus { get; set; }
        public EnrollmentRole EnrollmentRole { get; set; }

        public UserCourseEnrollmentInfo(Domain.Entities.CourseEnrollment courseEnrollment, string userAvatarInBase64)
        {
            UserId = courseEnrollment.UserId;
            CourseId = courseEnrollment.CourseId;

            UserAvatarInBase64 = userAvatarInBase64;

            UserFullName = courseEnrollment.User?.FullName ?? "Unknown";
            UserEmail = courseEnrollment.User?.Email ?? "Unknown";

            CourseName = courseEnrollment.Course?.Name ?? "Unknown";
        }
    }
}
