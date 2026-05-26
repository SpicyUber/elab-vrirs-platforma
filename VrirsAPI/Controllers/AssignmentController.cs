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
        public readonly IMediator mediator;

        public AssignmentController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [Authorize(Roles = "Teacher,Admin")]
        [HttpPost("create")]
        public async Task<ActionResult<Assignment>> Post([FromBody] Guid courseId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var result = await mediator.Send(new CreateAssignmentCommand()
            {
                CourseId = courseId,
                CreatedByUserId = userId
            });

            return Ok(result);
        }

        [Authorize(Roles = "Teacher,Admin,Student")]
        [HttpPost("course/{courseId}")]
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
