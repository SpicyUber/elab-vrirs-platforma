using Application.Commands.User;
using Application.DTOs.CourseEnrollment;
using Application.DTOs.User;
using Application.Queries.CourseEnrollment;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace VrirsAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator mediator;

        public UserController(IMediator mediator)
        {
            this.mediator = mediator;

        }

        [HttpGet("enrolled-in/{courseId}")]
        public async Task<ActionResult<List<UserCourseEnrollmentInfo>>> GetEnrolledUsers(Guid courseId)
        {
            var request = new GetAllCourseEnrollmentsByCourseIdQuery() { CourseId = courseId };

            var response = await mediator.Send(request);
            return Ok(response);
        }

        /*[HttpPut("avatar-upload")]
        public async Task<ActionResult<>>*/

        [HttpPost("register")]
        public async Task<ActionResult<UserSessionInfo>> Register([FromBody] RegisterCommand request)
        {
            try
            {
                var response = await mediator.Send(request);
                return Ok(response);
            }
            catch(InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserSessionInfo>> Login([FromBody] LoginCommand request)
        {
            try
            {
                var response = await mediator.Send(request);
                return Ok(response);
            }
            catch(UnauthorizedAccessException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
