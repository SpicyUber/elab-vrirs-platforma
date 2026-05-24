using Domain.Enums;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Course : IAuditableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
        public Guid CreatedByUserId { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public CourseCategory Category { get; set; }    

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public User CreatedByUser { get; set; } = null!;

        public ICollection<CourseEnrollment> Enrollments { get; set; } = [];
        public ICollection<Assignment> Assignments { get; set; } = [];
    }
}
