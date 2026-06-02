using Application.DTOs.SubmissionReview;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.SubmissionReview
{
    public class CreateSubmissionReviewCommand : IRequest<SubmissionReviewInfo>
    {
        public Guid? ReviewedByUserId { get; set; } = null;
        public Guid SubmissionId { get; set; }

        public ReviewStatus ReviewStatus { get; set; }

        public string? ReviewComment { get; set; }

        public int Points { get; set; }
    }
}
