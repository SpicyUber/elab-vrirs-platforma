using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Submission
{
    public class CreateSubmissionCommand : IRequest<Domain.Entities.Submission>
    {
        public Guid AssignmentId { get; set; }
        public Guid StudentUserId { get; set; }
    }
}
