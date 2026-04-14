using Application.Commands.Course;
using Application.DTOs.Course;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace VrirsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Teacher")]
    public class CourseController : ControllerBase
    {
        private readonly IMediator mediator;

        public CourseController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("create")]
        public async Task<ActionResult<CourseInfo>> CreateCourse([FromBody] CreateCourseInfo request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            try
            {
                var course = await mediator.Send(new CreateCourseCommand
                {
                    Name = request.Name,
                    Description = request.Description,
                    CreatedByUserId = userId
                });
                return Ok(course);
            }
            catch(InvalidOperationException e) { return BadRequest(e.Message); }
            
        }
    }
}
