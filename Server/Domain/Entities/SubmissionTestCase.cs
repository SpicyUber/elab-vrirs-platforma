using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class SubmissionTestCase
    {
        public Guid Id { get; set; }
        public Guid SubmissionTestId { get; set; }

        public SubmissionTest SubmissionTest { get; set; } = null!;
    }
}
