using Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string FullName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

       
        public ICollection<Course> CreatedCourses { get; set; } = [];
        public ICollection<CourseEnrollment> Enrollments { get; set; } = [];
        public ICollection<Assignment> CreatedAssignments { get; set; } = [];
        public ICollection<Submission> Submissions { get; set; } = [];
        public ICollection<SubmissionReview> Reviews { get; set; } = [];
        public ICollection<ProjectExecution> TriggeredExecutions { get; set; } = [];
    }
}
