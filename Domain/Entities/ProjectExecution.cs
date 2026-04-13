using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ProjectExecution
    {
        public Guid Id { get; set; }
        public Guid SubmissionId { get; set; }
        public Guid TriggeredByUserId { get; set; }
        public ExecutionStatus ExecutionStatus { get; set; } = ExecutionStatus.Pending;
        public DateTime? StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public string? OutputLog { get; set; }
        public string? ErrorLog { get; set; }

        public Submission Submission { get; set; } = null!;
        public User TriggeredByUser { get; set; } = null!;
    }
}
