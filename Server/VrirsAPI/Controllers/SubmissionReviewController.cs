using Application.Commands.SubmissionReview;
using Application.DTOs.SubmissionReview;
using Application.Queries.SubmissionReview;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace VrirsAPI.Controllers
{
    [Route("api/submissions")]
    [ApiController]
    public class SubmissionReviewController : ControllerBase
    {
        private readonly IMediator mediator;

        public SubmissionReviewController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("mine/reviews")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<List<SubmissionReviewInfo>>> GetAllByStudentId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await mediator.Send(new GetSubmissionReviewByUserIdQuery() { UserId = Guid.Parse(userId) });
            return Ok(result);
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost("{submissionId}/reviews")]
        public async Task<ActionResult<SubmissionReviewInfo>> PostReview(Guid submissionId ,[FromBody] PostReviewRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId == null) return Forbid();

            var command = new CreateSubmissionReviewCommand()
            {
                SubmissionId = submissionId,
                Points = request.Points,
                ReviewStatus = request.ReviewStatus,
                ReviewComment = request.ReviewComment,
                ReviewedByUserId = Guid.Parse(userId),
            };

            try
            {
                var result = await mediator.Send(command);
                return Ok(result);
            }
            catch(InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize]
        [HttpGet("{submissionId}/reviews")]
        public async Task<ActionResult<List<SubmissionReviewInfo>>> GetAllBySubmissionId(Guid submissionId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId == null) return Forbid();

            try
            {
                var result = await mediator.Send(new GetAllReviewsBySubmissionIdQuery() { SubmissionId = submissionId });
                return Ok(result);
            }
            catch(Exception)
            {
                return NotFound();
            }
        }
    }
}
