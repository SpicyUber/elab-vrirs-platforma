using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.User
{
    public class EnrollUsingUserIdRequest
    {
        public Guid UserId {get; set;}
        public bool IsTeacher {get; set;}
    }
}
