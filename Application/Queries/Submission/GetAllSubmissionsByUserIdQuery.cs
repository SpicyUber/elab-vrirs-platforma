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
    public class GetAllSubmissionsByUserIdQuery : IRequest<List<SubmissionInfo>>
    {
        public GetAllSubmissionsByUserIdQuery(Guid userId)
        {
            this.UserId = userId;
        }

        public Guid UserId { get; set; }
    }
}
