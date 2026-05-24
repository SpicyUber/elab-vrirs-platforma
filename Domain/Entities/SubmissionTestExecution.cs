using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class SubmissionTestExecution
    {
        public Guid Id { get; set; }

        public Guid SubmissionId { get; set; }
        public Guid SubmissionTestId { get; set; }

        public Guid? RanOnFileId { get; set; } = null;

        public Guid? TriggeredByUserId { get; set; }
        public ExecutionStatus ExecutionStatus { get; set; } = ExecutionStatus.Pending;

        public DateTime? StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }

        public string? OutputLog { get; set; }
        public string? ErrorLog { get; set; }

        public Submission Submission { get; set; } = null!;
        public SubmissionTest SubmissionTest { get; set; } = null!;

        public FileMetadata? RanOnFile { get; set; } = null;
        public User? TriggeredByUser { get; set; } = null;
    }
}
