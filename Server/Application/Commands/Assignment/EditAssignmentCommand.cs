using Application.DTOs.Assignment;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Assignment
{
    public class EditAssignmentCommand : IRequest<AssignmentInfo>
    {
        public Guid AssignmentId { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        public AssignmentLocation Location { get; set; }
        public AssignmentCategory Category { get; set; } = AssignmentCategory.Other;

        public DateTime? OpensAt { get; set; }
        public DateTime? DueAt { get; set; }

        public bool Publish { get; set; }

        public bool AllowProjectUpload { get; set; }
        public bool AllowMultipleAttempts { get; set; }

        public int MaxPoints { get; set; }
        public int MinPoints { get; set; }
    }
}
