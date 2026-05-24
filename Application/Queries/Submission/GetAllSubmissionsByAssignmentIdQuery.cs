using Application.DTOs.Submission;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Submission
{
    public class GetAllSubmissionsByAssignmentIdQuery : IRequest<List<SubmissionInfo>>
    {
        public GetAllSubmissionsByAssignmentIdQuery(Guid assignmentId) => AssignmentId = assignmentId;

        public Guid AssignmentId { get; set; }
    }
}
