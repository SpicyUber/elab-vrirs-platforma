using Domain.Enums;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class User : IdentityUser<Guid>, IAuditableEntity
    {
        public string FullName { get; set; } = string.Empty;
        public string? IndexNumber { get; set; } = null;
        public bool IsActive { get; set; } = true;
        public Guid? AvatarFileId { get; set; } = null;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public FileMetadata? AvatarFile { get; set; } = null;
        public ICollection<Course> CreatedCourses { get; set; } = [];
        public ICollection<CourseEnrollment> Enrollments { get; set; } = [];
        public ICollection<Assignment> CreatedAssignments { get; set; } = [];
        public ICollection<Submission> Submissions { get; set; } = [];
        public ICollection<SubmissionReview> Reviews { get; set; } = [];
        public ICollection<SubmissionTestExecution> TriggeredExecutions { get; set; } = [];
    }
}
