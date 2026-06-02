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
        [HttpPost("course/{courseId}")]
        public async Task<ActionResult<Assignment>> Post(Guid courseId)
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
        [HttpPut("course/{courseId}")]
        public async Task<ActionResult<Assignment>> Edit([FromBody] EditAssignmentCommand request)
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

        [HttpGet("course/{courseId}/mine")]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<List<AssignmentInfo>>> GetAllByUserId()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            return Ok(
                await mediator.Send
                (
                    new GetAllAssignmentsByUserIdQuery()
                    {
                        UserId = userId
                    }
                )
            );
        }

        [Authorize(Roles = "Teacher,Admin,Student")]
        [HttpGet("course/{courseId}")]
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
    }
}
