using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Course
{
    public class CourseInfo
    {
        public Guid Id { get; set; } = Guid.Empty;

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public Guid CreatedByUserId { get; set; } = Guid.Empty;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public CourseCategory Category { get; set; }

        public CourseInfo(Domain.Entities.Course course)
        {
            this.Id = course.Id;
            this.Name = course.Name;
            this.Description = course.Description;
            this.CreatedByUserId = course.CreatedByUserId;
            this.StartDate = course.StartDate;
            this.EndDate = course.EndDate;
            this.Category = course.Category;
        }
    }
}
