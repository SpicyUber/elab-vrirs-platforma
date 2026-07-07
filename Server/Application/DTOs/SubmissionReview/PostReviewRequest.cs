using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.SubmissionReview
{
    public class PostReviewRequest
    {
        public ReviewStatus ReviewStatus { get; set; }

        public string? ReviewComment { get; set; }

        public int Points { get; set; }
    }
}
