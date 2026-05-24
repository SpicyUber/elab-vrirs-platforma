using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;


namespace Infrastructure.Persistence
{
    public class VrirsDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public VrirsDbContext(DbContextOptions<VrirsDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseEnrollment> CourseEnrollments { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<AssignmentAsset> AssignmentAssets { get; set; }
        public DbSet<FileMetadata> FileMetadata { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<ProjectAsset> ProjectAssets { get; set; }
        public DbSet<SubmissionReview> SubmissionReviews { get; set; }
        public DbSet<SubmissionTest> SubmissionTests { get; set; }
        public DbSet<SubmissionTestCase> SubmissionTestCases { get; set; }
        public DbSet<SubmissionTestExecution> SubmissionTestExecutions { get; set; }

        public override int SaveChanges()
        {
            ApplyAuditTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAuditTimestamps()
        {
            var now = DateTime.UtcNow;
            foreach(var entry in ChangeTracker.Entries())
            {
                if(entry.Entity is IAuditableEntity auditableEntity)
                {
                    if(entry.State == EntityState.Added) auditableEntity.CreatedAt = now;
                    if(entry.State is EntityState.Added or EntityState.Modified) auditableEntity.UpdatedAt = now;
                }

            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultSchema("vrirs");

            ConfigureUser(builder);
            ConfigureCourse(builder);

            ConfigureCourseEnrollment(builder);
            ConfigureAssignment(builder);

            ConfigureAssignmentAsset(builder);
            ConfigureFileMetadata(builder);

            ConfigureSubmission(builder);
            ConfigureProjectAsset(builder);

            ConfigureSubmissionReview(builder);
            ConfigureSubmissionTestCase(builder);

            ConfigureSubmissionTestExecution(builder);

            AddRoles(builder);
        }

        private void ConfigureUser(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(u => u.AvatarFile)
                .WithMany()
                .HasForeignKey(u => u.AvatarFileId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);
        }

        private void ConfigureCourse(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>()
                .HasOne(c => c.CreatedByUser)
                .WithMany(u => u.CreatedCourses)
                .HasForeignKey(c => c.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void ConfigureCourseEnrollment(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CourseEnrollment>()
                .HasKey(e => new { e.CourseId, e.UserId });

            modelBuilder.Entity<CourseEnrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId);

            modelBuilder.Entity<CourseEnrollment>()
                .HasOne(e => e.User)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void ConfigureAssignment(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Course)
                .WithMany(c => c.Assignments)
                .HasForeignKey(a => a.CourseId);

            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.CreatedByUser)
                .WithMany(u => u.CreatedAssignments)
                .HasForeignKey(a => a.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.SubmissionTest)
                .WithMany()
                .HasForeignKey(a => a.SubmissionTestId)
                .IsRequired(false);

            modelBuilder.Entity<Assignment>()
                .ToTable(t => t.HasCheckConstraint("CK_Assignment_Points",
        "[MinPoints] >= 0 AND [MaxPoints] >= [MinPoints]"));
        }

        private void ConfigureAssignmentAsset(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AssignmentAsset>()
                .HasKey(a => new { a.AssignmentId, a.FileMetadataId });

            modelBuilder.Entity<AssignmentAsset>()
                .HasOne(a => a.Assignment)
                .WithMany(a => a.AssignmentAssets)
                .HasForeignKey(a => a.AssignmentId);

            modelBuilder.Entity<AssignmentAsset>()
                .HasOne(a => a.FileMetadata)
                .WithMany()
                .HasForeignKey(a => a.FileMetadataId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void ConfigureFileMetadata(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileMetadata>()
                .HasOne(f => f.UploadedByUser)
                .WithMany()
                .HasForeignKey(f => f.UploadedByUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void ConfigureSubmission(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Submission>()
                .HasOne(s => s.Assignment)
                .WithMany(a => a.Submissions)
                .HasForeignKey(s => s.AssignmentId);

            modelBuilder.Entity<Submission>()
                .HasOne(s => s.Student)
                .WithMany(u => u.Submissions)
                .HasForeignKey(s => s.StudentUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void ConfigureProjectAsset(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProjectAsset>()
                .HasKey(p => new { p.SubmissionId, p.FileMetadataId });

            modelBuilder.Entity<ProjectAsset>()
                .HasOne(p => p.Submission)
                .WithMany(s => s.Assets)
                .HasForeignKey(p => p.SubmissionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectAsset>()
                .HasOne(p => p.FileMetadata)
                .WithMany()
                .HasForeignKey(p => p.FileMetadataId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void ConfigureSubmissionReview(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SubmissionReview>()
                .HasOne(r => r.Submission)
                .WithMany(s => s.Reviews)
                .HasForeignKey(r => r.SubmissionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SubmissionReview>()
                .HasOne(r => r.ReviewedByUser)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.ReviewedByUserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);
        }

        private void ConfigureSubmissionTestCase(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SubmissionTestCase>()
                .HasOne(tc => tc.SubmissionTest)
                .WithMany(st => st.TestCases)
                .HasForeignKey(tc => tc.SubmissionTestId);
        }

        private void ConfigureSubmissionTestExecution(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SubmissionTestExecution>()
                .HasOne(e => e.Submission)
                .WithMany(s => s.TestExecutions)
                .HasForeignKey(e => e.SubmissionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SubmissionTestExecution>()
                .HasOne(e => e.SubmissionTest)
                .WithMany()
                .HasForeignKey(e => e.SubmissionTestId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SubmissionTestExecution>()
                .HasOne(e => e.RanOnFile)
                .WithMany()
                .HasForeignKey(e => e.RanOnFileId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<SubmissionTestExecution>()
                .HasOne(e => e.TriggeredByUser)
                .WithMany(u => u.TriggeredExecutions)
                .HasForeignKey(e => e.TriggeredByUserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);
        }

        private void AddRoles(ModelBuilder builder)
        {
            var studentRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var teacherRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var adminRoleId = Guid.Parse("33333333-3333-3333-3333-333333333333");

            builder.Entity<IdentityRole<Guid>>().HasData(
                new IdentityRole<Guid>
                {
                    Id = studentRoleId,
                    Name = "Student",
                    NormalizedName = "STUDENT"
                },
                new IdentityRole<Guid>
                {
                    Id = teacherRoleId,
                    Name = "Teacher",
                    NormalizedName = "TEACHER"
                },
                new IdentityRole<Guid>
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                }
            );
        }
    }
}
