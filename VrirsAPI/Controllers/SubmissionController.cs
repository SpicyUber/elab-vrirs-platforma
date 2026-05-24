using Application.Commands.Submission;
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
        public readonly IMediator mediator;

        public SubmissionController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [Authorize(Roles = "Student,Teacher,Admin")]
        [HttpGet("mine")]
        public async Task<ActionResult<List<Submission>>> GetAllByUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId == null) return Forbid();

            return Ok(await mediator.Send(new GetAllSubmissionsByUserIdQuery(Guid.Parse(userId))));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<ActionResult<List<Submission>>> GetAll()
        {
            return Ok(await mediator.Send(new GetAllSubmissionsQuery()));
        }

        [Authorize(Roles = "Student,Teacher,Admin")]
        [HttpGet("assignment/{assignmentId}")]
        public async Task<ActionResult<List<Submission>>> GetAllByAssignmentId(Guid assignmentId)
        {
            return Ok(await mediator.Send(new GetAllSubmissionsByAssignmentIdQuery(assignmentId)));
        }

        [Authorize(Roles = "Student")]
        [HttpPost("create")]
        public async Task<ActionResult<Submission>> Post([FromBody] Guid assignmentId)
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
    }
}
