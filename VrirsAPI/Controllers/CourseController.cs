using Application.Commands.Course;
using Application.Commands.CourseEnrollment;
using Application.DTOs.Course;
using Application.DTOs.CourseEnrollment;
using Application.DTOs.User;
using Application.Queries.Course;
using Application.Queries.CourseEnrollment;
using Application.Queries.Submission;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
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

        [HttpGet("{courseId}/enrolled-users")]
        public async Task<ActionResult<List<UserCourseEnrollmentInfo>>> GetEnrolledUsers(Guid courseId)
        {
            var request = new GetAllCourseEnrollmentsByCourseIdQuery() { CourseId = courseId };

            var response = await mediator.Send(request);
            return Ok(response);
        }

        [Authorize(Roles = "Teacher,Admin")]
        [Consumes("multipart/form-data")]
        [HttpPost("{courseId}/enrolled-users/by-csv")]
        public async Task<ActionResult<List<UserCourseEnrollmentInfo>>> EnrollUsers(Guid courseId, IFormFile emailCsv)
        {
            try
            {
                using Stream csvStream = emailCsv.OpenReadStream();
                var request = new EnrollUsingEmailCsvCommand() { CourseId = courseId, EmailCsv = csvStream };

                var response = await mediator.Send(request);
                return Ok(response);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Teacher,Admin")]
        [HttpPost("{courseId}/enrolled-users/by-id")]
        public async Task<ActionResult<UserCourseEnrollmentInfo>> EnrollUsers(Guid courseId, [FromBody] EnrollUsingUserIdRequest request)
        {
            try
            {
                var command = new EnrollUsingUserIdCommand() { CourseId = courseId, UserId = request.UserId, IsTeacher = request.IsTeacher };

                var response = await mediator.Send(request);
                return Ok(response);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Teacher,Admin")]
        [HttpDelete("{courseId}/enrolled-users/by-id")]
        public async Task<ActionResult> UnenrollUsers(Guid courseId, [FromBody] Guid userId)
        {
            try
            {
                var command = new UnenrollUsingUserIdCommand() { UserId = userId, CourseId = courseId };
                await mediator.Send(command);
                return NoContent();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{courseId}")]
        public async Task<ActionResult> Archive(Guid courseId)
        {
            try
            {
                var command = new ArchiveCourseCommand() { CourseId = courseId };
                await mediator.Send(command);
                return NoContent();
            }
            catch(Exception)
            {
                return NotFound();
            }
        }
    }
}
