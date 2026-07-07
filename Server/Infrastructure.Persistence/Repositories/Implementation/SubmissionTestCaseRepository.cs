using Domain.Entities;
using Infrastructure.Persistence.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories.Implementation
{
    public class SubmissionTestCaseRepository : GenericRepository<SubmissionTestCase>, ISubmissionTestCaseRepository
    {
        public SubmissionTestCaseRepository(VrirsDbContext context) : base(context)
        {
        }
    }
}
