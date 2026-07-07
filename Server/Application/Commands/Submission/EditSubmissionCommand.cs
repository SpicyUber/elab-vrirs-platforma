using Application.DTOs.Submission;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Submission
{
    public class EditSubmissionCommand : IRequest<SubmissionInfo>
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool Publish { get; set; }
    }
}
