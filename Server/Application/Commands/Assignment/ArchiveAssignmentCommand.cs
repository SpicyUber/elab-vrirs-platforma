using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Assignment
{
    public class ArchiveAssignmentCommand : IRequest
    {
        public Guid AssignmentId { get; set; }
    }
}
