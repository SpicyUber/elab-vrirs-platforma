using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.User
{
    public class UserSearchResultPage
    {
        public List<UserProfileInfo> UserProfiles { get; set; }
        public int EntiresPerPage { get; set; }
        public int PageNumer {  get; set; }
        public int MaxPages { get; set; }
    }
}
