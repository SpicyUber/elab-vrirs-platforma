using Domain.Enums;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Assignment : IAuditableEntity
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }

        public Guid CreatedByUserId { get; set; }
        public Guid? SubmissionTestId { get; set; } = null;

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        public AssignmentLocation Location { get; set; }
        public AssignmentCategory Category { get; set; } = AssignmentCategory.Other;

        public DateTime? OpensAt { get; set; }
        public DateTime? DueAt { get; set; }

        public AssignmentStatus Status { get; set; } = AssignmentStatus.Draft;

        public bool AllowProjectUpload { get; set; }
        public bool AllowMultipleAttempts { get; set; }

        public int MaxPoints { get; set; }
        public int MinPoints { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Course Course { get; set; } = null!;
        public User CreatedByUser { get; set; } = null!;

        public SubmissionTest? SubmissionTest { get; set; } = null;

        public ICollection<Submission> Submissions { get; set; } = [];
        public ICollection<AssignmentAsset> AssignmentAssets { get; set; } = [];
    }
}
