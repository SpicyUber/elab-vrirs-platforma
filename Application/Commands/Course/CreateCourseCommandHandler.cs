using Application.DTOs.Course;
using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
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

        public CreateCourseCommandHandler(IUnitOfWork uow) { this.uow = uow; }
        public Task<CourseInfo> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {

            if (string.IsNullOrEmpty(request.Name)) throw new InvalidOperationException("Name is required.");
            var course = new Domain.Entities.Course
            {
                Name = request.Name,
                Description = request.Description,
                CreatedByUserId = request.CreatedByUserId,
                IsActive = true,
            };

            uow.CourseRepository.Add(course);
            uow.SaveChanges();

            return Task.FromResult(new CourseInfo(course));
        }
    }
}
