using Application.DTOs.Submission;
using MediatR;

namespace Application.Commands.Submission
{
    public class CreateSubmissionCommand : IRequest<SubmissionInfo>
    {
        public Guid AssignmentId { get; set; }
        public Guid StudentUserId { get; set; }
    }
}
