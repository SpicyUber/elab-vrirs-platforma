using Infrastructure.Persistence.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.UnitOfWork.Interface
{
    public interface IUnitOfWork
        {
            public IAssignmentRepository AssignmentRepository { get; }
            public ICourseEnrollmentRepository CourseEnrollmentRepository { get; }
            public ICourseRepository CourseRepository { get; }
            public IProjectAssetRepository ProjectAssetRepository { get; }
            public IProjectExecutionRepository ProjectExecutionRepository { get; }
            public ISubmissionContentRepository SubmissionContentRepository { get; }
            public ISubmissionRepository SubmissionRepository { get; }
            public ISubmissionReviewRepository SubmissionReviewRepository { get; }
            public IUserRepository UserRepository { get; }

            public int SaveChanges();
        }


    
}
