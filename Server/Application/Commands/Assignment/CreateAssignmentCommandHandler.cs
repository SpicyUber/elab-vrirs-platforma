using Application.Commands.Submission;
using Application.DTOs.Assignment;
using Domain.Enums;
using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Assignment
{
    public class CreateAssignmentCommandHandler : IRequestHandler<CreateAssignmentCommand, AssignmentInfo>
    {
        private readonly IUnitOfWork uow;

        public CreateAssignmentCommandHandler(IUnitOfWork uow) { this.uow = uow; }

        public async Task<AssignmentInfo> Handle(CreateAssignmentCommand request, CancellationToken cancellationToken)
        {
            bool isTeacher = await uow.CourseEnrollmentRepository.Query()
                                                                 .Where(ce => ce.UserId == request.CreatedByUserId
                                                                 && ce.CourseId == request.CourseId
                                                                 && ce.EnrollmentRole == EnrollmentRole.Teacher)
                                                                 .AnyAsync(cancellationToken);

            if(!isTeacher) throw new InvalidOperationException("Must be a teacher on this course!");

            var newAssignment = new Domain.Entities.Assignment()
            {
                CourseId = request.CourseId,
                CreatedByUserId = request.CreatedByUserId,
                Status = Domain.Enums.AssignmentStatus.Draft,
            };

            uow.AssignmentRepository.Add(newAssignment);
            await uow.SaveChangesAsync(cancellationToken);

            newAssignment = await uow.AssignmentRepository.Query().Include(a => a.Course).Include(a => a.CreatedByUser).FirstAsync(a => a.Id == newAssignment.Id);

            return new AssignmentInfo(newAssignment);
        }
    }
}
