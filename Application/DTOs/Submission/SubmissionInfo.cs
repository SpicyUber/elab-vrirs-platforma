using Domain.Enums;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Submission
{
    public class SubmissionInfo
    {
        public Guid Id { get; set; }
        public Guid AssignmentId { get; set; }
        public Guid StudentUserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public SubmissionStatus Status { get; set; } = SubmissionStatus.Draft;
        public DateTime? SubmittedAt { get; set; }

        public SubmissionInfo(Domain.Entities.Submission submission)
        {
            Id = submission.Id;
            AssignmentId = submission.AssignmentId;
            StudentUserId = submission.StudentUserId;
            Title = submission.Title;
            Description = submission.Description;
            Status = submission.Status;
            SubmittedAt = submission.SubmittedAt;
        }
    }
}
