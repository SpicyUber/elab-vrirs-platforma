using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Assignment
{
    public class ArchiveAssignmentCommandHandler : IRequestHandler<ArchiveAssignmentCommand>
    {
        private readonly IUnitOfWork uow;
        public ArchiveAssignmentCommandHandler(IUnitOfWork uow) => this.uow = uow;

        public async Task Handle(ArchiveAssignmentCommand request, CancellationToken cancellationToken)
        {
            var assignment = uow.AssignmentRepository.GetById(request.AssignmentId);
            if(assignment == null) throw new InvalidOperationException("Could not find assignment.");

            assignment.Status = Domain.Enums.AssignmentStatus.Archived;

            await uow.SaveChangesAsync(cancellationToken);
        }
    }
}
