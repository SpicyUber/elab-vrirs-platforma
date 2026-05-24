using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Submission
{
    public class CreateSubmissionCommandHandler : IRequestHandler<CreateSubmissionCommand, Domain.Entities.Submission>
    {
        private readonly IUnitOfWork uow;

        public CreateSubmissionCommandHandler(IUnitOfWork uow) { this.uow = uow; }

        public async Task<Domain.Entities.Submission> Handle(CreateSubmissionCommand request, CancellationToken cancellationToken)
        {
            var newSubmission = new Domain.Entities.Submission()
            {
                StudentUserId = request.StudentUserId,
                AssignmentId = request.AssignmentId,
                Status = Domain.Enums.SubmissionStatus.Draft
            };

            uow.SubmissionRepository.Add(newSubmission);

            uow.SaveChanges();

            return newSubmission;
        }
    }
}
