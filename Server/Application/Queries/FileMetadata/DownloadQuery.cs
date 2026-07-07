using Application.DTOs.FileMetadata;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.FileMetadata
{
    public class DownloadQuery : IRequest<DownloadPackage>
    {
        public Guid FileId { get; set; }
    }
}
