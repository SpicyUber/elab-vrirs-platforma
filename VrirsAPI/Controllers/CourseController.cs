using Application.Commands.Course;
using Application.DTOs.Course;
using Application.Queries.Course;
using Application.Queries.Submission;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace VrirsAPI.Controllers
{
    [Route("api/courses")]
    [ApiController]

    public class CourseController : ControllerBase
    {
        private readonly IMediator mediator;

        public CourseController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [Authorize(Roles = "Teacher,Admin")]
        [HttpPost("create")]
        public async Task<ActionResult<CourseInfo>> Post([FromBody] CreateCourseInfo request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            try
            {
                var course = await mediator.Send(new CreateCourseCommand
                {
                    Name = request.Name,
                    Description = request.Description,
                    CreatedByUserId = userId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Category = request.Category
                });
                return Ok(course);
            }
            catch(InvalidOperationException e) { return BadRequest(e.Message); }

        }

        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<ActionResult<List<CourseInfo>>> GetAll()
        {
            return Ok(await mediator.Send(new GetAllCoursesQuery()));
        }

        [Authorize(Roles = "Teacher,Student,Admin")]
        [HttpGet("mine")]
        public async Task<ActionResult<List<CourseInfo>>> GetAllByUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId == null) return Forbid();

            return Ok(await mediator.Send(new GetAllCoursesByUserIdQuery() { UserId = Guid.Parse(userId) }));
        }

    }
}
