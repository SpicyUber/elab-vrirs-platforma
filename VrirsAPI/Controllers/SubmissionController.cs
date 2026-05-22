using Application.Queries.Submission;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace VrirsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubmissionController : ControllerBase
    {
        public readonly IMediator mediator;

        public SubmissionController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [Authorize(Roles = "Student,Teacher,Admin")]
        [HttpGet]
        public async Task<ActionResult<List<Submission>>> Get()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId == null) return Forbid();

            return Ok(await mediator.Send(new GetAllSubmissionsByUserIdQuery(Guid.Parse(userId))));
        }
    }
}
