using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.User
{
    public class UserProfileInfo
    {
        public string FullName { get; set; }
        public string Email { get; set; }

        public string? IndexNumber { get; set; }
        public string AvatarInBase64 { get; set; }

        public string Role { get; set; }

        public UserProfileInfo(Domain.Entities.User user, string avatarInBase64, string role) 
        {
            FullName = user.FullName;
            Email = user.Email;

            IndexNumber = user.IndexNumber;
            AvatarInBase64 = avatarInBase64;

            Role = role;
        }
    }
}
