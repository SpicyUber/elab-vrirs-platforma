using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Course
{
    public class CreateCourseInfo
    {
        
        public string Name { get; set; } = string.Empty;

        
        public string? Description { get; set; }
    }
}
