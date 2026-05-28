using Application.DTOs.CourseEnrollment;
using Domain.Entities;
using Infrastructure.Persistence.FileService.Interfaces;
using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.CourseEnrollment
{
    public class GetAllCourseEnrollmentsByCourseIdQueryHandler : IRequestHandler<GetAllCourseEnrollmentsByCourseIdQuery, List<UserCourseEnrollmentInfo>>
    {
        private readonly IUnitOfWork uow;
        private readonly IFileService<Stream> fileService;

        public GetAllCourseEnrollmentsByCourseIdQueryHandler(IUnitOfWork uow, IFileService<Stream> fileService)
        {
            this.uow = uow;
            this.fileService = fileService;
        }

        public async Task<List<UserCourseEnrollmentInfo>> Handle(GetAllCourseEnrollmentsByCourseIdQuery request, CancellationToken cancellationToken)
        {
            var courseEnrollments = await uow.CourseEnrollmentRepository.Query()
                                   .Include(ce => ce.User)
                                   .Include(ce => ce.Course)
                                   .Include(ce => ce.User.AvatarFile)
                                   .Where(ce => ce.CourseId == request.CourseId)
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

        private async Task<string> GetAvatarInBase64(User user, CancellationToken cancellationToken)
        {
            if(user?.AvatarFile?.StoragePath == null) return null;

            using Stream stream = await fileService.DownloadAsync(user.AvatarFile.StoragePath, cancellationToken);
            using MemoryStream memoryStream = new();

            await stream.CopyToAsync(memoryStream, cancellationToken);

            return Convert.ToBase64String(memoryStream.ToArray());
        }
    }
}
