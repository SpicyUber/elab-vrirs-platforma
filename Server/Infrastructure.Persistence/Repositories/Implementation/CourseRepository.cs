using Domain.Entities;
using Infrastructure.Persistence.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories.Implementation
{
    public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {
        public CourseRepository(VrirsDbContext context) : base(context)
        {
        }
    }
}
