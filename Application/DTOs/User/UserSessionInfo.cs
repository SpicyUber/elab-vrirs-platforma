using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.DTOs.User
{
    public class UserSessionInfo
    {
        public Guid Id { get; set; }

        public string FullName { get; set; }
        public string Email { get; set; }

        public string Roles { get; set; }
        public string Token { get; set; }

        public UserSessionInfo(Domain.Entities.User user, string roles, string token)
        {
            Id = user.Id;

            FullName = user.FullName;
            Email = user.Email;

            Token = token;
            Roles = roles;
        }
    }
}
