using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class SubmissionReview
    {
        public Guid Id { get; set; }
        public Guid SubmissionId { get; set; }
        public Guid ReviewedByUserId { get; set; }
        public ReviewStatus ReviewStatus { get; set; }
        public string? ReviewComment { get; set; }
        public DateTime ReviewedAt { get; set; }

        public Submission Submission { get; set; } = null!;
        public User ReviewedByUser { get; set; } = null!;
    }
}
