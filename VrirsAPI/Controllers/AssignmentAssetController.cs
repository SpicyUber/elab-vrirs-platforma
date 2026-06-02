using Application.Commands.AssignmentAsset;
using Application.Commands.ProjectAsset;
using Application.DTOs.AssignmentAsset;
using Application.DTOs.ProjectAsset;
using Application.Queries.AssignmentAsset;
using Application.Queries.SubmissionReview;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace VrirsAPI.Controllers
{
    [Route("api/assignments")]
    [ApiController]
    public class AssignmentAssetController : ControllerBase
    {
        private readonly IMediator mediator;

        public AssignmentAssetController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [Authorize]
        [HttpGet("{assignmentId}/assets")]
        public async Task<ActionResult<List<AssignmentAssetInfo>>> GetAllByAssignmentId(Guid assignmentId) 
        {
            try
            {
                var result = await mediator.Send(new GetAllAssignmentAssetsByAssignmentIdQuery() { Id = assignmentId });
                return Ok(result);
            }
            catch(Exception)
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "Teacher")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(100_000_000)]
        [HttpPost("{assignmentId}/assets")]
        public async Task<ActionResult<AssignmentAssetInfo>> Post(IFormFile asset,Guid assignmentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId == null) return Forbid();

            try
            {
                using Stream uploadStream = asset.OpenReadStream();

                UploadAssignmentAssetCommand request = new()
                {
                    AssetUploadStream = uploadStream,

                    AssignmentId = assignmentId,
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
    }
}
