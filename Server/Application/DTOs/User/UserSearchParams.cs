using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.User
{
    public class UserSearchParams
    {
        public string? FullName { get; set; }
        public string? Index { get; set; }
        public string? Email { get; set; }

        public int PageNumber {  get; set; }
        public int EntriesPerPage { get; set; }
    }
}
