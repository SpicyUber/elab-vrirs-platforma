using Application.DTOs.AssignmentAsset;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.AssignmentAsset
{
    public class GetAllAssignmentAssetsByAssignmentIdQuery : IRequest<List<AssignmentAssetInfo>>
    {
        public Guid Id { get; set; }
    }
}
