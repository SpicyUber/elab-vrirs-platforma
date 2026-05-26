using Application.DTOs.Submission;
using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands.Submission
{
    public class CreateSubmissionCommandHandler : IRequestHandler<CreateSubmissionCommand, SubmissionInfo>
    {
        private readonly IUnitOfWork uow;

        public CreateSubmissionCommandHandler(IUnitOfWork uow) { this.uow = uow; }

        public async Task<SubmissionInfo> Handle(CreateSubmissionCommand request, CancellationToken cancellationToken)
        {
            var newSubmission = new Domain.Entities.Submission()
            {
                StudentUserId = request.StudentUserId,
                AssignmentId = request.AssignmentId,
                Status = Domain.Enums.SubmissionStatus.Draft
            };

            uow.SubmissionRepository.Add(newSubmission);
            await uow.SaveChangesAsync();

            newSubmission = await uow.SubmissionRepository.Query().Include(s => s.Student).Include(s => s.Assignment).FirstAsync(s => s.Id == newSubmission.Id);

            return new SubmissionInfo(newSubmission);
        }
    }
}
