using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
 

namespace Infrastructure.Persistence
{
    public class VrirsDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public VrirsDbContext(DbContextOptions<VrirsDbContext> options) : base(options) { }

        public DbSet<Course> Courses => Set<Course>();
        public DbSet<CourseEnrollment> CourseEnrollments => Set<CourseEnrollment>();
        public DbSet<Assignment> Assignments => Set<Assignment>();
        public DbSet<Submission> Submissions => Set<Submission>();
        public DbSet<SubmissionContent> SubmissionContents => Set<SubmissionContent>();
        public DbSet<ProjectAsset> ProjectAssets => Set<ProjectAsset>();
        public DbSet<SubmissionReview> SubmissionReviews => Set<SubmissionReview>();
        public DbSet<ProjectExecution> ProjectExecutions => Set<ProjectExecution>();

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
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is User u)
                {
                    if (entry.State == EntityState.Added) u.CreatedAt = now;
                    if (entry.State is EntityState.Added or EntityState.Modified) u.UpdatedAt = now;
                }
                else if (entry.Entity is Course c)
                {
                    if (entry.State == EntityState.Added) c.CreatedAt = now;
                    if (entry.State is EntityState.Added or EntityState.Modified) c.UpdatedAt = now;
                }
                else if (entry.Entity is Assignment a)
                {
                    if (entry.State == EntityState.Added) a.CreatedAt = now;
                    if (entry.State is EntityState.Added or EntityState.Modified) a.UpdatedAt = now;
                }
                else if (entry.Entity is Submission s)
                {
                    if (entry.State == EntityState.Added) s.CreatedAt = now;
                    if (entry.State is EntityState.Added or EntityState.Modified) s.UpdatedAt = now;
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultSchema("vrirs");


            builder.Entity<User>().ToTable("users");
            builder.Entity<IdentityRole<Guid>>().ToTable("roles");
            builder.Entity<IdentityUserRole<Guid>>().ToTable("user_roles");
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("user_claims");
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("user_logins");
            builder.Entity<IdentityUserToken<Guid>>().ToTable("user_tokens");
            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("role_claims");


            builder.Entity<User>(e =>
            {
                e.Property(u => u.Id).HasDefaultValueSql("NEWSEQUENTIALID( )");
                e.Property(u => u.FullName).IsRequired().HasMaxLength(200);
                
                e.Property(u => u.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(u => u.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

          
            builder.Entity<Course>(e =>
            {
                e.ToTable("courses");
                e.HasKey(c => c.Id);
                e.Property(c => c.Id).HasDefaultValueSql("NEWSEQUENTIALID( )");
                e.Property(c => c.Name).IsRequired().HasMaxLength(300);
                e.Property(c => c.Description).HasMaxLength(2000);
                e.Property(c => c.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(c => c.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

                e.HasOne(c => c.CreatedByUser)
                 .WithMany(u => u.CreatedCourses)
                 .HasForeignKey(c => c.CreatedByUserId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            
            builder.Entity<CourseEnrollment>(e =>
            {
                e.ToTable("course_enrollments");
                e.HasKey(ce => ce.Id);
                e.Property(ce => ce.Id).HasDefaultValueSql("NEWSEQUENTIALID( )");
                e.Property(ce => ce.EnrollmentRole).HasConversion<string>().HasMaxLength(20);
                e.Property(ce => ce.Status).HasConversion<string>().HasMaxLength(20);
                e.Property(ce => ce.EnrolledAt).HasDefaultValueSql("GETUTCDATE()");

                e.HasIndex(ce => new { ce.CourseId, ce.UserId }).IsUnique();

                e.HasOne(ce => ce.Course)
                 .WithMany(c => c.Enrollments)
                 .HasForeignKey(ce => ce.CourseId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(ce => ce.User)
                 .WithMany(u => u.Enrollments)
                 .HasForeignKey(ce => ce.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

             
            builder.Entity<Assignment>(e =>
            {
                e.ToTable("assignments");
                e.HasKey(a => a.Id);
                e.Property(a => a.Id).HasDefaultValueSql("NEWSEQUENTIALID( )");
                e.Property(a => a.Title).IsRequired().HasMaxLength(400);
                e.Property(a => a.Description).HasMaxLength(4000);
                e.Property(a => a.Status).HasConversion<string>().HasMaxLength(20);
                e.Property(a => a.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(a => a.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

                e.HasOne(a => a.Course)
                 .WithMany(c => c.Assignments)
                 .HasForeignKey(a => a.CourseId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(a => a.CreatedByUser)
                 .WithMany(u => u.CreatedAssignments)
                 .HasForeignKey(a => a.CreatedByUserId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

             
            builder.Entity<Submission>(e =>
            {
                e.ToTable("submissions");
                e.HasKey(s => s.Id);
                e.Property(s => s.Id).HasDefaultValueSql("NEWSEQUENTIALID( )");
                e.Property(s => s.Title).IsRequired().HasMaxLength(400);
                e.Property(s => s.Description).HasMaxLength(4000);
                e.Property(s => s.SubmissionType).HasConversion<string>().HasMaxLength(20);
                e.Property(s => s.Status).HasConversion<string>().HasMaxLength(30);
                e.Property(s => s.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(s => s.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

                e.HasOne(s => s.Assignment)
                 .WithMany(a => a.Submissions)
                 .HasForeignKey(s => s.AssignmentId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(s => s.Student)
                 .WithMany(u => u.Submissions)
                 .HasForeignKey(s => s.StudentUserId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

             
            builder.Entity<SubmissionContent>(e =>
            {
                e.ToTable("submission_contents");
                e.HasKey(sc => sc.Id);
                e.Property(sc => sc.Id).HasDefaultValueSql("NEWSEQUENTIALID( )");
                e.Property(sc => sc.ContentFormat).HasConversion<string>().HasMaxLength(20);
                e.Property(sc => sc.ContentText).IsRequired();
                e.Property(sc => sc.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                e.HasIndex(sc => new { sc.SubmissionId, sc.VersionNumber }).IsUnique();

                e.HasOne(sc => sc.Submission)
                 .WithMany(s => s.Contents)
                 .HasForeignKey(sc => sc.SubmissionId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

             
            builder.Entity<ProjectAsset>(e =>
            {
                e.ToTable("project_assets");
                e.HasKey(pa => pa.Id);
                e.Property(pa => pa.Id).HasDefaultValueSql("NEWSEQUENTIALID( )");
                e.Property(pa => pa.AssetType).HasConversion<string>().HasMaxLength(30);
                e.Property(pa => pa.FileName).IsRequired().HasMaxLength(500);
                e.Property(pa => pa.FilePath).IsRequired().HasMaxLength(1000);
                e.Property(pa => pa.MimeType).IsRequired().HasMaxLength(100);
                e.Property(pa => pa.UploadStatus).HasConversion<string>().HasMaxLength(20);
                e.Property(pa => pa.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                e.HasOne(pa => pa.Submission)
                 .WithMany(s => s.Assets)
                 .HasForeignKey(pa => pa.SubmissionId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

             
            builder.Entity<SubmissionReview>(e =>
            {
                e.ToTable("submission_reviews");
                e.HasKey(sr => sr.Id);
                e.Property(sr => sr.Id).HasDefaultValueSql("NEWSEQUENTIALID( )");
                e.Property(sr => sr.ReviewStatus).HasConversion<string>().HasMaxLength(30);
                e.Property(sr => sr.ReviewComment).HasMaxLength(4000);
                e.Property(sr => sr.ReviewedAt).HasDefaultValueSql("GETUTCDATE()");

                e.HasOne(sr => sr.Submission)
                 .WithMany(s => s.Reviews)
                 .HasForeignKey(sr => sr.SubmissionId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(sr => sr.ReviewedByUser)
                 .WithMany(u => u.Reviews)
                 .HasForeignKey(sr => sr.ReviewedByUserId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

           
            builder.Entity<ProjectExecution>(e =>
            {
                e.ToTable("project_executions");
                e.HasKey(pe => pe.Id);
                e.Property(pe => pe.Id).HasDefaultValueSql("NEWSEQUENTIALID( )");
                e.Property(pe => pe.ExecutionStatus).HasConversion<string>().HasMaxLength(20);
                e.Property(pe => pe.StartedAt);

                e.HasOne(pe => pe.Submission)
                 .WithMany(s => s.Executions)
                 .HasForeignKey(pe => pe.SubmissionId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(pe => pe.TriggeredByUser)
                 .WithMany(u => u.TriggeredExecutions)
                 .HasForeignKey(pe => pe.TriggeredByUserId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            var studentRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var teacherRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");

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
                }
            );
        }
    }
}
