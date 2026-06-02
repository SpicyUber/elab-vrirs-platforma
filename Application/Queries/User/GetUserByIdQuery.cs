using Application.DTOs.User;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.User
{
    public class GetUserByIdQuery : IRequest<UserProfileInfo>
    {
        public Guid Id { get; set; }
    }
}
