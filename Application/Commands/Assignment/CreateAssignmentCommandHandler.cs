using Application.Commands.Submission;
using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Assignment
{
    public class CreateAssignmentCommandHandler : IRequestHandler<CreateAssignmentCommand, Domain.Entities.Assignment>
    {
        private readonly IUnitOfWork uow;

        public CreateAssignmentCommandHandler(IUnitOfWork uow) { this.uow = uow; }

        public async Task<Domain.Entities.Assignment> Handle(CreateAssignmentCommand request, CancellationToken cancellationToken)
        {
            var newAssignment = new Domain.Entities.Assignment()
            {
                CourseId = request.CourseId,
                CreatedByUserId = request.CreatedByUserId,
                Status = Domain.Enums.AssignmentStatus.Draft,
            };

            uow.AssignmentRepository.Add(newAssignment);

            uow.SaveChanges();

            return newAssignment;
        }
    }
}
