using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class CourseEnrollment
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public Guid UserId { get; set; }
        public EnrollmentRole EnrollmentRole { get; set; }
        public DateTime EnrolledAt { get; set; }
        public EnrollmentStatus Status { get; set; }

        public Course Course { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
