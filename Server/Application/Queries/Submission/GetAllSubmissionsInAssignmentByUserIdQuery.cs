using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Application.DTOs.Submission;

namespace Application.Queries.Submission
{
    public class GetAllSubmissionsInAssignmentByUserIdQuery : IRequest<List<SubmissionInfo>>
    {
        public GetAllSubmissionsInAssignmentByUserIdQuery(Guid userId, Guid assignmentId)
        {
            UserId = userId;
            AssignmentId = assignmentId;
        }

        public Guid UserId { get; set; }
        public Guid AssignmentId { get; set; }
    }
}
