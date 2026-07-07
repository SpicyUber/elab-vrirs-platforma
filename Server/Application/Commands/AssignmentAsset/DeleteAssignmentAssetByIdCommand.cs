using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.AssignmentAsset
{
    public class DeleteAssignmentAssetByIdCommand : IRequest
    {
        public Guid AssignmentId { get; set; }
        public Guid FileMetadataId { get; set; }
    }
}
