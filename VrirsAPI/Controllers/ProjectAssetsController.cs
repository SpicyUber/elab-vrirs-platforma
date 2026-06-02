using Application.Commands.ProjectAsset;
using Application.DTOs.ProjectAsset;
using Application.Queries.SubmissionReview;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace VrirsAPI.Controllers
{
    [Route("api/submissions")]
    [ApiController]
    public class ProjectAssetsController : ControllerBase
    {
        private readonly IMediator mediator;

        public ProjectAssetsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [Authorize(Roles = "Student")]
        [Consumes("multipart/form-data")]
        [HttpPost("{submissionId}/assets")]
        public async Task<ActionResult<ProjectAssetInfo>> PostAsset(IFormFile asset, Guid submissionId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId == null) return Forbid();

            try
            {
                using Stream uploadStream = asset.OpenReadStream();

                UploadProjectAssetCommand request = new()
                {
                    AssetUploadStream = uploadStream,

                    SubmissionId = submissionId,
                    UserId = Guid.Parse(userId),

                    Name = Path.GetFileNameWithoutExtension(asset.FileName),
                    Extension = Path.GetExtension(asset.FileName),

                    SizeInBytes = asset.Length,
                    Mime = asset.ContentType
                };

                var response = await mediator.Send(request);

                return Ok(response);
            }
            catch(InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize]
        [HttpGet("{submissionId}/assets")]
        public async Task<ActionResult<ProjectAssetInfo>> GetAllBySubmissionId(Guid submissionId)
        {
            try
            {
                var result = await mediator.Send(new GetAllReviewsBySubmissionIdQuery() { Id = submissionId });
                return Ok(result);
            }
            catch(Exception)
            {
                return NotFound();
            }
        }
    }
}
