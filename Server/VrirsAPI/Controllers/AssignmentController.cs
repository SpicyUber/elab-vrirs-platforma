using Application.Commands.Assignment;
using Application.DTOs.Assignment;
using Application.Queries.Assignment;
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
    public class AssignmentController : ControllerBase
    {
        private readonly IMediator mediator;

        public AssignmentController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [Authorize(Roles = "Teacher,Admin")]
        [HttpPost("from-course/{courseId}")]
        public async Task<ActionResult<AssignmentInfo>> Post(Guid courseId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await mediator.Send(new CreateAssignmentCommand()
            {
                CourseId = courseId,
                CreatedByUserId = userId
            });

            return Ok(result);
        }

        [Authorize(Roles = "Teacher")]
        [HttpPut("from-course/{courseId}")]
        public async Task<ActionResult<AssignmentInfo>> Edit([FromBody] EditAssignmentCommand request)
        {
            try
            {
                var result = await mediator.Send(request);
                return Ok(result);
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("from-course/{courseId}/mine")]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<List<AssignmentInfo>>> GetAllByUserId(Guid courseId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            return Ok(
                await mediator.Send
                (
                    new GetAllAssignmentsByUserIdQuery()
                    {
                        UserId = userId,
                        CourseId = courseId
                    }
                )
            );
        }

        [Authorize(Roles = "Teacher,Admin,Student")]
        [HttpGet("{assignmentId}")]
        public async Task<ActionResult<AssignmentInfo>> GetById(Guid assignmentId)
        {
            return Ok(await mediator.Send(
                new GetAssignmentByIdQuery()
                {
                    AssignmentId = assignmentId
                }));
        }

        [Authorize(Roles = "Teacher,Admin,Student")]
        [HttpGet("from-course/{courseId}")]
        public async Task<ActionResult<List<AssignmentInfo>>> GetAllByCourseId(Guid courseId)
        {
            return Ok(
                await mediator.Send
                (
                    new GetAllAssignmentsByCourseIdQuery()
                    {
                        CourseId = courseId
                    }
                )
            );
        }

        [Authorize(Roles = "Teacher,Admin")]
        [HttpDelete("{assignmentId}")]
        public async Task<ActionResult<List<AssignmentInfo>>> ArchiveById(Guid assignmentId)
        {
            try
            {
                await mediator.Send
                    (
                        new ArchiveAssignmentCommand()
                        {
                            AssignmentId = assignmentId
                        }
                    );
                return NoContent();
            }
            catch(Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
