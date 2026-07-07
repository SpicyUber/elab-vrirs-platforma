using Application.Commands.Submission;
using Application.DTOs.Submission;
using Application.Queries.Submission;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<ActionResult<List<SubmissionInfo>>> GetAllFromAssignmentByUserId(Guid assignmentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId == null) return Forbid();

            return Ok(await mediator.Send(new GetAllSubmissionsInAssignmentByUserIdQuery(Guid.Parse(userId), assignmentId)));
        }

        [Authorize(Roles = "Student")]
        [HttpGet("mine")]
        public async Task<ActionResult<List<SubmissionInfo>>> GetAllByUserId(Guid assignmentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId == null) return Forbid();

            return Ok(await mediator.Send(new GetAllSubmissionsByUserIdQuery() { UserId = Guid.Parse(userId) }));
        }

        [Authorize]
        [HttpGet("{submissionId}")]
        public async Task<ActionResult<SubmissionInfo>> GetFromAssignmentById(Guid submissionId)
        {
            try
            {
                return Ok(await mediator.Send(new GetSubmissionByIdQuery() {SubmissionId = submissionId}));
            }
            catch(Exception ex)
            {
                return NotFound(ex.Message);
            }
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId == null) return Forbid();

            request.UserId = Guid.Parse(userId);

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
