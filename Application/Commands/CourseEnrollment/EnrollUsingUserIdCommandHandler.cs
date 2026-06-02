using Application.DTOs.CourseEnrollment;
using Domain.Enums;
using Infrastructure.Persistence.FileService.Interfaces;
using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands.CourseEnrollment
{
    public class EnrollUsingUserIdCommandHandler : IRequestHandler<EnrollUsingUserIdCommand, UserCourseEnrollmentInfo>
    {
        private readonly IUnitOfWork uow;
        private readonly IFileService<Stream> fileService;

        public EnrollUsingUserIdCommandHandler(IUnitOfWork uow, IFileService<Stream> fileService)
        {
            this.fileService = fileService;
            this.uow = uow;
        }

        public async Task<UserCourseEnrollmentInfo> Handle(EnrollUsingUserIdCommand request, CancellationToken cancellationToken)
        {
            Domain.Entities.CourseEnrollment enrollment;

            try
            {
                enrollment = Enroll(request);
                await uow.SaveChangesAsync(cancellationToken);
            }
            catch(Exception)
            {
                throw new InvalidOperationException("Error enrolling user.");
            }

            var user = await uow.UserRepository.Query()
                                               .Where(u => u.Id == request.UserId)
                                               .Include(u => u.AvatarFile)
                                               .FirstAsync(cancellationToken);

            string avatarInBase64 = await GetAvatarInBase64(user, cancellationToken);

            return new(enrollment, avatarInBase64);
        }

        private async Task<string> GetAvatarInBase64(Domain.Entities.User user, CancellationToken cancellationToken)
        {
            if(user?.AvatarFile?.StoragePath == null) return null;

            using Stream stream = await fileService.DownloadAsync(user.AvatarFile.StoragePath, cancellationToken);
            using MemoryStream memoryStream = new();

            await stream.CopyToAsync(memoryStream, cancellationToken);
            return Convert.ToBase64String(memoryStream.ToArray());
        }

        private Domain.Entities.CourseEnrollment Enroll(EnrollUsingUserIdCommand request)
        {
            var enrollment =
                            new Domain.Entities.CourseEnrollment()
                            {
                                UserId = request.UserId,
                                EnrolledAt = DateTime.UtcNow,
                                Status = Domain.Enums.EnrollmentStatus.Active,
                                CourseId = request.CourseId,
                                EnrollmentRole = request.IsTeacher ? EnrollmentRole.Teacher : EnrollmentRole.Student
                            };

            uow.CourseEnrollmentRepository.Add(enrollment);

            return enrollment;
        }
    }
}
