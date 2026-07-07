using Application.DTOs.Submission;
using Domain.Enums;
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
            Guid courseId = uow.AssignmentRepository.GetById(request.AssignmentId).CourseId;

            bool isStudent = await uow.CourseEnrollmentRepository.Query()
                                                                 .Where(ce => ce.UserId == request.StudentUserId
                                                                 && ce.CourseId == courseId
                                                                 && ce.EnrollmentRole == EnrollmentRole.Student)
                                                                 .AnyAsync(cancellationToken);

            if(!isStudent) throw new InvalidOperationException("Must be a student on this course!");

            var assignment  = await uow.AssignmentRepository.Query().Where(a => a.Id == request.AssignmentId).FirstOrDefaultAsync(cancellationToken);

            if(assignment == null) throw new InvalidOperationException("Assignment not found!");

            bool openNow = (assignment.OpensAt == null || assignment.OpensAt <= DateTime.UtcNow) && (assignment.DueAt == null || assignment.DueAt >= DateTime.UtcNow);

            if(!openNow) throw new InvalidOperationException("Assignment closed!");

            bool isResubmission = await uow.SubmissionRepository.Query()
                                                                .Where(s => s.AssignmentId == request.AssignmentId 
                                                                && s.StudentUserId == request.StudentUserId)
                                                                .AnyAsync(cancellationToken);

            if(isResubmission && !assignment.AllowMultipleAttempts) throw new InvalidOperationException("Resubmission not allowed!");

            var newSubmission = new Domain.Entities.Submission()
            {
                StudentUserId = request.StudentUserId,
                AssignmentId = request.AssignmentId,
                Status = Domain.Enums.SubmissionStatus.Draft
            };

            uow.SubmissionRepository.Add(newSubmission);
            await uow.SaveChangesAsync(cancellationToken);

            newSubmission =
                await uow.SubmissionRepository
                .Query()
                .Include(s => s.Student)
                .Include(s => s.Assignment)
                .FirstAsync(s => s.Id == newSubmission.Id);

            return new SubmissionInfo(newSubmission);
        }
    }
}
