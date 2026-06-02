using Application.Commands.ProjectAsset;
using Application.Commands.Submission;
using Application.Commands.SubmissionReview;
using Application.Commands.User;
using Application.DTOs.ProjectAsset;
using Application.DTOs.Submission;
using Application.DTOs.SubmissionReview;
using Application.Queries.Submission;
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
    public class SubmissionController : ControllerBase
    {
        private readonly IMediator mediator;

        public SubmissionController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [Authorize(Roles = "Student")]
        [HttpGet("from-assignment/{assignmentId}/mine")]
        public async Task<ActionResult<List<SubmissionInfo>>> GetAllByUserId(Guid assignmentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId == null) return Forbid();

            return Ok(await mediator.Send(new GetAllSubmissionsByUserIdQuery(Guid.Parse(userId), assignmentId)));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<ActionResult<List<SubmissionInfo>>> GetAll()
        {
            return Ok(await mediator.Send(new GetAllSubmissionsQuery()));
        }

        [Authorize(Roles = "Student,Teacher,Admin")]
        [HttpGet("from-assignment/{assignmentId}")]
        public async Task<ActionResult<List<SubmissionInfo>>> GetAllByAssignmentId(Guid assignmentId)
        {
            return Ok(await mediator.Send(new GetAllSubmissionsByAssignmentIdQuery(assignmentId)));
        }

        [Authorize(Roles = "Student")]
        [HttpPost("from-assignment/{assignmentId}")]
        public async Task<ActionResult<SubmissionInfo>> Post(Guid assignmentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId == null) return Forbid();

            var request = new CreateSubmissionCommand()
            {
                StudentUserId = Guid.Parse(userId),
                AssignmentId = assignmentId
            };

            return Ok(await mediator.Send(request));

        }

        [Authorize(Roles = "Student")]
        [HttpPut]
        public async Task<ActionResult<SubmissionInfo>> Put([FromBody] EditSubmissionCommand request)
        {
            try
            {
                return Ok(await mediator.Send(request));
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
