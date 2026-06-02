using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Course
{
    public class ArchiveCourseCommandHandler : IRequestHandler<ArchiveCourseCommand>
    {
        private readonly IUnitOfWork uow;
        public ArchiveCourseCommandHandler(IUnitOfWork uow) { this.uow = uow; }

        public async Task Handle(ArchiveCourseCommand request, CancellationToken cancellationToken)
        {
            var course = await uow.CourseRepository.Query().Where(c => c.Id == request.CourseId).FirstAsync(cancellationToken);
            course.IsActive = false;
            await uow.SaveChangesAsync(cancellationToken);
        }
    }
}
