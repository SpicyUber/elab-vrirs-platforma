using Application.DTOs.AssignmentAsset;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.AssignmentAsset
{
    public class UploadAssignmentAssetCommand : IRequest<AssignmentAssetInfo>
    {
        public Stream AssetUploadStream { get; set; }
        public Guid AssignmentId { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public long SizeInBytes { get; set; }
        public string Mime { get; set; }
    }
}
