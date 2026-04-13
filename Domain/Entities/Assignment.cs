using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Assignment
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? OpensAt { get; set; }
        public DateTime? DueAt { get; set; }
        public AssignmentStatus Status { get; set; } = AssignmentStatus.Draft;
        public bool AllowProjectUpload { get; set; }
        public bool AllowMultipleAttempts { get; set; }
        public Guid CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Course Course { get; set; } = null!;
        public User CreatedByUser { get; set; } = null!;
        public ICollection<Submission> Submissions { get; set; } = [];
    }
}
