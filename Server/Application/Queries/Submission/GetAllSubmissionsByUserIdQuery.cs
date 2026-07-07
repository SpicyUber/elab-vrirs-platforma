using Application.DTOs.Submission;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Submission
{
    public class GetAllSubmissionsByUserIdQuery : IRequest<List<SubmissionInfo>>
    {
        public Guid UserId { get; set; }
    }
}
