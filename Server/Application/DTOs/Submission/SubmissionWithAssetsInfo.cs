using Application.DTOs.ProjectAsset;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Submission
{
    public class SubmissionWithAssetsInfo
    {
        public Guid Id { get; set; }

        public Guid AssignmentId { get; set; }
        public Guid StudentUserId { get; set; }

        public string AssignmentTitle { get; set; }
        public string StudentFullName { get; set; }

        public string? StudentIndex { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        public SubmissionStatus Status { get; set; } = SubmissionStatus.Draft;
        public DateTime? SubmittedAt { get; set; }

        public List<ProjectAssetInfo> Assets { get; set; }

        public SubmissionWithAssetsInfo(Domain.Entities.Submission submission)
        {
            Id = submission.Id;
            AssignmentId = submission.AssignmentId;

            StudentUserId = submission.StudentUserId;
            AssignmentTitle = submission.Assignment?.Title ?? "Unknown";

            StudentFullName = submission.Student?.FullName ?? "Unknown";
            StudentIndex = submission.Student?.IndexNumber;

            Title = submission.Title;
            Description = submission.Description;

            Status = submission.Status;
            SubmittedAt = submission.SubmittedAt;

            Assets = new();

            foreach(Domain.Entities.ProjectAsset asset in submission.Assets)
                Assets.Add(new(asset));
        }
    }
}
