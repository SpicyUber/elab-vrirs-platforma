using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Assignment
{
    public class CreateAssignmentCommand : IRequest<Domain.Entities.Assignment>
    {
        public Guid CourseId { get; set; }
        public Guid CreatedByUserId { get; set; }
    }
}
