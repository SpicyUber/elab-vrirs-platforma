using Application.DTOs.Course;
using Domain.Entities;
using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Course
{
    public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, CourseInfo>
    {
        private readonly IUnitOfWork uow;
        private readonly UserManager<Domain.Entities.User> userManager;

        public CreateCourseCommandHandler(IUnitOfWork uow, UserManager<Domain.Entities.User> userManager)
        {
            this.uow = uow;
            this.userManager = userManager;
        }
        public async Task<CourseInfo> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {

            if(string.IsNullOrEmpty(request.Name)) throw new InvalidOperationException("Name is required.");
            var course = new Domain.Entities.Course
            {
                Name = request.Name,
                Description = request.Description,
                CreatedByUserId = request.CreatedByUserId,
                IsActive = true,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Category = request.Category,
            };

            uow.CourseRepository.Add(course);

            await uow.SaveChangesAsync(cancellationToken);

            if((await userManager.GetRolesAsync(uow.UserRepository.GetById(request.CreatedByUserId))).Contains("Teacher"))
            {
                var courseEnrollment =
                    new Domain.Entities.CourseEnrollment()
                    {
                        CourseId = course.Id,
                        UserId = request.CreatedByUserId,
                        EnrolledAt = DateTime.UtcNow,
                        Status = Domain.Enums.EnrollmentStatus.Active,
                        EnrollmentRole = Domain.Enums.EnrollmentRole.Teacher
                    };

                uow.CourseEnrollmentRepository.Add(courseEnrollment);
            }

            await uow.SaveChangesAsync(cancellationToken);

            course = await uow.CourseRepository.Query().Include(c => c.CreatedByUser).FirstAsync(c => c.Id == course.Id);

            return new CourseInfo(course);
        }
    }
}
