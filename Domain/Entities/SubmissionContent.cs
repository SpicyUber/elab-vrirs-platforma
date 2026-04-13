using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class SubmissionContent
    {
        public Guid Id { get; set; }
        public Guid SubmissionId { get; set; }
        public ContentFormat ContentFormat { get; set; }
        public string ContentText { get; set; } = string.Empty;
        public int VersionNumber { get; set; }
        public DateTime CreatedAt { get; set; }

        public Submission Submission { get; set; } = null!;
    }
}
