using Domain.Entities;
using Infrastructure.Persistence.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories.Implementation
{
    public class AssignmentRepository : GenericRepository<Assignment> , IAssignmentRepository
    {
        public AssignmentRepository(VrirsDbContext context) : base(context)
        {
        }
    }
}
