using Application.DTOs.User;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.User
{
    public class EditUserCommand : IRequest<UserProfileInfo>
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string? IndexNumber { get; set; }
    }
}
