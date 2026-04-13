using Domain.Entities;
using Infrastructure.Persistence.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories.Implementation
{
    public class SubmissionContentRepository : GenericRepository<SubmissionContent>, ISubmissionContentRepository
    {
        public SubmissionContentRepository(VrirsDbContext context) : base(context)
        {
        }
    }
}
