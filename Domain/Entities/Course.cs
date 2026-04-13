using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Course
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid CreatedByUserId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public User CreatedByUser { get; set; } = null!;
        public ICollection<CourseEnrollment> Enrollments { get; set; } = [];
        public ICollection<Assignment> Assignments { get; set; } = [];
    }
}
