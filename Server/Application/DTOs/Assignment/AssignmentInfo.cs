using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Assignment
{
    public class AssignmentInfo
    {
        public Guid Id { get; set; }

        public Guid CourseId { get; set; }
        public string CourseName { get; set; }

        public Guid CreatedByUserId { get; set; }
        public string CreatedByUserFullName { get; set; }

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

        public AssignmentInfo(Domain.Entities.Assignment assignment)
        {
            Id = assignment.Id;

            CourseId = assignment.CourseId;
            CourseName = assignment.Course?.Name ?? "Unknown";

            CreatedByUserId = assignment.CreatedByUserId;
            CreatedByUserFullName = assignment.CreatedByUser?.FullName ?? "Unknown";

            SubmissionTestId = assignment.SubmissionTestId;

            Title = assignment.Title;
            Description = assignment.Description;

            Location = assignment.Location;
            Category = assignment.Category;

            OpensAt = assignment.OpensAt;
            DueAt = assignment.DueAt;

            Status = assignment.Status;

            AllowProjectUpload = assignment.AllowProjectUpload;
            AllowMultipleAttempts = assignment.AllowMultipleAttempts;

            MaxPoints = assignment.MaxPoints;
            MinPoints = assignment.MinPoints;
        }
    }
}
