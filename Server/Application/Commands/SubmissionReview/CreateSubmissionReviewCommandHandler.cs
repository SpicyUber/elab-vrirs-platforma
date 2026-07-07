using Application.DTOs.SubmissionReview;
using Domain.Enums;
using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands.SubmissionReview
{
    public class CreateSubmissionReviewCommandHandler : IRequestHandler<CreateSubmissionReviewCommand, SubmissionReviewInfo>
    {
        private readonly IUnitOfWork uow;

        public CreateSubmissionReviewCommandHandler(IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public async Task<SubmissionReviewInfo> Handle(CreateSubmissionReviewCommand request, CancellationToken cancellationToken)
        {
            Guid courseId = (await uow.SubmissionRepository.Query()
                                                           .Include(s => s.Assignment)
                                                           .Where(s => s.Id == request.SubmissionId)
                                                           .FirstAsync(cancellationToken))
                                                           .Assignment
                                                           .CourseId;

            bool isTeacher = await uow.CourseEnrollmentRepository.Query()
                                                                 .Where(ce => ce.UserId == request.ReviewedByUserId
                                                                 && ce.CourseId == courseId
                                                                 && ce.EnrollmentRole == EnrollmentRole.Teacher)
                                                                 .AnyAsync(cancellationToken);

            if(!isTeacher) throw new InvalidOperationException("Must be a teacher on this course!");

            var review = new Domain.Entities.SubmissionReview()
            {
                SubmissionId = request.SubmissionId,
                ReviewedByUserId = request.ReviewedByUserId,
                ReviewedAt = DateTime.UtcNow,
                ReviewStatus = request.ReviewStatus,
                Points = request.Points,
                ReviewComment = request.ReviewComment
            };

            int maxPoints =
                await uow.SubmissionRepository
                      .Query()
                      .Include(s => s.Assignment)
                      .Where(s => s.Id == request.SubmissionId)
                      .Select(s => s.Assignment.MaxPoints)
                      .FirstAsync(cancellationToken);

            if(request.Points > maxPoints)
                throw new InvalidOperationException("Points cannot exceed assignment max points!");

            uow.SubmissionReviewRepository.Add(review);

            await uow.SaveChangesAsync(cancellationToken);

            return new(review);
        }
    }
}
