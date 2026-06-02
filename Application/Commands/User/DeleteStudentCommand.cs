using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.User
{
    public class DeleteStudentCommand : IRequest
    {
        public Guid StudentId { get; set; }
    }
}
