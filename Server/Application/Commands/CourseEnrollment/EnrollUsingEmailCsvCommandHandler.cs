using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;
using Application.DTOs.CourseEnrollment;
using Application.DTOs.User;
using CsvHelper;
using Domain.Enums;
using Infrastructure.Persistence.FileService.Interfaces;
using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;


namespace Application.Commands.CourseEnrollment
{
    public class EnrollUsingEmailCsvCommandHandler : IRequestHandler<EnrollUsingEmailCsvCommand, List<UserCourseEnrollmentInfo>>
    {
        private readonly IUnitOfWork uow;
        private readonly IFileService<Stream> fileService;
        private readonly List<Guid> newEnrollmentUserIds;

        public EnrollUsingEmailCsvCommandHandler(IUnitOfWork uow, IFileService<Stream> fileService)
        {
            this.uow = uow;
            this.fileService = fileService;
            newEnrollmentUserIds = new();
        }

        public async Task<List<UserCourseEnrollmentInfo>> Handle(EnrollUsingEmailCsvCommand request, CancellationToken cancellationToken)
        {
            List<UserEnrollmentRow> rows;
            rows = ParseUserEmails(request.EmailCsv);

            await TryEnrollAllUsersFromEmails(request.CourseId, rows, cancellationToken);

            var courseEnrollments = await uow.CourseEnrollmentRepository.Query()
                                               .Include(ce => ce.User)
                                               .Include(ce => ce.Course)
                                               .Include(ce => ce.User.AvatarFile)
                                               .Where(ce => newEnrollmentUserIds.Contains(ce.UserId))
                                               .ToListAsync(cancellationToken);

            var courseEnrollmentsWithAvatars = await Task.WhenAll(
                courseEnrollments.Select(async ce =>
                    new UserCourseEnrollmentInfo(
                        ce,
                        await GetAvatarInBase64(ce.User, cancellationToken
                        )
                    )
                )
            );

            return courseEnrollmentsWithAvatars.ToList();
        }

        private List<UserEnrollmentRow> ParseUserEmails(Stream EmailCsv)
        {
            List<UserEnrollmentRow> rows;

            try
            {
                using var reader = new StreamReader(EmailCsv);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                rows = csv.GetRecords<UserEnrollmentRow>().ToList();
            }
            catch(Exception)
            {
                throw new ValidationException("Csv is not structured properly!");
            }

            return rows;
        }

        private async Task TryEnrollAllUsersFromEmails(Guid courseId, List<UserEnrollmentRow> rows, CancellationToken cancellationToken)
        {
            try
            {
                foreach(var row in rows.GroupBy(r => r.Email).Select(g => g.First()))
                {
                    await EnrollUser(row.Email, row.IsTeacher, courseId, cancellationToken);
                }

                await uow.SaveChangesAsync(cancellationToken);
            }
            catch(Exception)
            {
                throw new InvalidOperationException("Error while enrolling users!");
            }
        }

        private async Task<string> GetAvatarInBase64(Domain.Entities.User user, CancellationToken cancellationToken)
        {
            if(user?.AvatarFile?.StoragePath == null) return null;

            using Stream stream = await fileService.DownloadAsync(user.AvatarFile.StoragePath, cancellationToken);
            using MemoryStream memoryStream = new();

            await stream.CopyToAsync(memoryStream, cancellationToken);
            return Convert.ToBase64String(memoryStream.ToArray());
        }

        private async Task EnrollUser(string email, bool isTeacher, Guid courseId, CancellationToken cancellationToken)
        {
            var user = await uow.UserRepository.Query()
                                               .Where(u => u.Email == email)
                                               .FirstOrDefaultAsync(cancellationToken);

            if(user == null) return;

            var enrollment =
                new Domain.Entities.CourseEnrollment()
                {
                    UserId = user.Id,
                    EnrolledAt = DateTime.UtcNow,
                    Status = Domain.Enums.EnrollmentStatus.Active,
                    CourseId = courseId,
                    EnrollmentRole = isTeacher ? EnrollmentRole.Teacher : EnrollmentRole.Student
                };

            uow.CourseEnrollmentRepository.Add(enrollment);
            newEnrollmentUserIds.Add(enrollment.UserId);
        }
    }
}
