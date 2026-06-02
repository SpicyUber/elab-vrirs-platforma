using Application.DTOs.ProjectAsset;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.ProjectAsset
{
    public class GetAllProjectAssetsBySubmissionIdQuery : IRequest<List<ProjectAssetInfo>>
    {
        public Guid Id { get; set; }
    }
}
