using Infrastructure.Persistence.Repositories.Implementation;
using Infrastructure.Persistence.Repositories.Interfaces;
using Infrastructure.Persistence.UnitOfWork.Interface;

namespace Infrastructure.Persistence.UnitOfWork.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly VrirsDbContext context;

        public IAssignmentRepository AssignmentRepository { get; }
        public ICourseEnrollmentRepository CourseEnrollmentRepository { get; }

        public ICourseRepository CourseRepository { get; }
        public IProjectAssetRepository ProjectAssetRepository { get; }

        public ISubmissionTestExecutionRepository ProjectExecutionRepository { get; }
        public ISubmissionRepository SubmissionRepository { get; }

        public ISubmissionReviewRepository SubmissionReviewRepository { get; }
        public IUserRepository UserRepository { get; }

        public ISubmissionTestCaseRepository SubmissionTestCaseRepository { get; }
        public ISubmissionTestRepository SubmissionTestRepository { get; }

        public IAssignmentAssetRepository AssignmentAssetRepository { get; }
        public IFileMetadataRepository FileMetadataRepository { get; }

        public UnitOfWork(VrirsDbContext context)
        {
            this.context = context;

            AssignmentRepository = new AssignmentRepository(this.context);
            CourseEnrollmentRepository = new CourseEnrollmentRepository(this.context);

            CourseRepository = new CourseRepository(this.context);
            ProjectAssetRepository = new ProjectAssetRepository(this.context);

            ProjectExecutionRepository = new SubmissionTestExecutionRepository(this.context);
            SubmissionRepository = new SubmissionRepository(this.context);

            SubmissionReviewRepository = new SubmissionReviewRepository(this.context);
            UserRepository = new UserRepository(this.context);

            SubmissionTestCaseRepository = new SubmissionTestCaseRepository(this.context);
            SubmissionTestRepository = new SubmissionTestRepository(this.context);
            
            AssignmentAssetRepository = new AssignmentAssetRepository(this.context);
            FileMetadataRepository = new FileMetadataRepository(this.context);
        }

        public int SaveChanges() => context.SaveChanges();
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken) => await context.SaveChangesAsync(cancellationToken);
    }
}
