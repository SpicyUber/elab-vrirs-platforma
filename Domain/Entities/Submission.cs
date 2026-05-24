using Domain.Enums;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Submission : IAuditableEntity
    {
        public Guid Id { get; set; }
        public Guid AssignmentId { get; set; }

        public Guid StudentUserId { get; set; }
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }
        public SubmissionStatus Status { get; set; } = SubmissionStatus.Draft;

        public DateTime? SubmittedAt { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Assignment Assignment { get; set; } = null!;
        public User Student { get; set; } = null!;

        public ICollection<ProjectAsset> Assets { get; set; } = [];
        public ICollection<SubmissionReview> Reviews { get; set; } = [];

        public ICollection<SubmissionTestExecution> TestExecutions { get; set; } = [];

    }
}
