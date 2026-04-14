using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Course
{
    public class CourseInfo
    {
        public string Name { get; set; } = string.Empty;


        public string? Description { get; set; }

        public string CreatedByUserId { get; set; } = string.Empty;

        public CourseInfo(Domain.Entities.Course course)
        {
            this.Name = course.Name;
            this.Description = course.Description;
            this.CreatedByUserId = course.CreatedByUserId.ToString();
        }
    }
}
