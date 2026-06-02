using Application.DTOs.ProjectAsset;
using Application.DTOs.Submission;
using MediatR;

namespace Application.Commands.ProjectAsset
{
    public class UploadProjectAssetCommand : IRequest<ProjectAssetInfo>
    {
        public Guid SubmissionId { get; set; }
        public Guid UserId { get; set; }

        public Stream AssetUploadStream { get; set; }
        public string Mime { get; set; }

        public string Extension { get; set; }

        public string Name { get; set; }

        public long SizeInBytes { get; set; }
    }
}
