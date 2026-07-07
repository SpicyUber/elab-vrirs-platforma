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
    public class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand, CourseInfo>
    {
        private readonly IUnitOfWork uow;
        public UpdateCourseCommandHandler(IUnitOfWork uow) => this.uow = uow;

        public async Task<CourseInfo> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
        {
            var course = uow.CourseRepository.GetById(request.Id);
            if(course == null)throw new InvalidOperationException("Invalid course id.");

            if(string.IsNullOrEmpty(request.Name)) throw new InvalidOperationException("Name is required.");
            course.Name = request.Name;
            course.Description = request.Description;
            course.StartDate = request.StartDate;
            course.EndDate = request.EndDate;
            course.Category = request.Category;

            await uow.SaveChangesAsync(cancellationToken);

            return new CourseInfo(course);
        }
    }
}
