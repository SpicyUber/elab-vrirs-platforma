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
        public string FullName { get; set;} 
        public string Email { get; set;}

        public string Role { get; set;}

        public string Token { get; set; }

        public UserSessionInfo(Domain.Entities.User user,string role, string token)
        {
            FullName= user.FullName;
            Email = user.Email;
            Token = token;
            Role = role;

        }
    }
}
