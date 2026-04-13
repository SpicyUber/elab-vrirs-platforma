using Infrastructure.Persistence.Repositories.Implementation;
using Infrastructure.Persistence.Repositories.Interfaces;
using Infrastructure.Persistence.UnitOfWork.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.UnitOfWork.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly VrirsDbContext context;
        public IAssignmentRepository AssignmentRepository { get; }
        public ICourseEnrollmentRepository CourseEnrollmentRepository { get; }
        public ICourseRepository CourseRepository { get; }
        public IProjectAssetRepository ProjectAssetRepository { get; }
        public IProjectExecutionRepository ProjectExecutionRepository { get; }
        public ISubmissionContentRepository SubmissionContentRepository { get; }
        public ISubmissionRepository SubmissionRepository { get; }
        public ISubmissionReviewRepository SubmissionReviewRepository { get; }
        public IUserRepository UserRepository { get; }
        public UnitOfWork(VrirsDbContext context)
        {
            this.context = context;

            AssignmentRepository = new AssignmentRepository(this.context);
            CourseEnrollmentRepository = new CourseEnrollmentRepository(this.context);
            CourseRepository = new CourseRepository(this.context);
            ProjectAssetRepository = new ProjectAssetRepository(this.context);
            ProjectExecutionRepository = new ProjectExecutionRepository(this.context);
            SubmissionContentRepository = new SubmissionContentRepository(this.context);
            SubmissionRepository = new SubmissionRepository(this.context);
            SubmissionReviewRepository = new SubmissionReviewRepository(this.context);
            UserRepository = new UserRepository(this.context);
        }

        public int SaveChanges()
        {
            throw new NotImplementedException();
        }
    }
}
