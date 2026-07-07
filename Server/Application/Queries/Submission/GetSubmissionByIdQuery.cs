using Application.DTOs.Submission;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Submission
{
    public class GetSubmissionByIdQuery : IRequest<SubmissionInfo>
    {
        public Guid SubmissionId { get; set; }
    }
}
