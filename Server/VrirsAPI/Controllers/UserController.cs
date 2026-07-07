using Application.Commands.User;
using Application.DTOs.User;
using Application.Queries.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

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

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserProfileInfo>> Get(Guid userId)
        {
            var request = new GetUserByIdQuery() { Id = userId };

            try
            {
                return Ok(await mediator.Send(request));
            }
            catch(Exception ex)
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpGet("search")]
        public async Task<ActionResult<UserSearchResultPage>> Get([FromQuery]UserSearchParams searchParams)
        {
            var request = new SearchUsersQuery() { SearchParams = searchParams };

            try
            {
                return Ok(await mediator.Send(request));
            }
            catch(Exception)
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{userId}")]
        public async Task<ActionResult> Delete(Guid userId)
        {
            try
            {
                await mediator.Send(new DeleteStudentCommand() { StudentId = userId });
                return Ok();
            }
            catch(Exception ex) { return BadRequest(ex.Message); }
        }

        [Authorize]
        [Consumes("multipart/form-data")]
        [HttpPost("avatar-upload")]
        public async Task<ActionResult<string>> UploadAvatar(IFormFile avatar)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            try
            {
                using Stream uploadStream = avatar.OpenReadStream();

                UploadAvatarCommand request = new()
                {
                    AvatarUploadStream = uploadStream,
                    UserId = userId,

                    Name = Path.GetFileNameWithoutExtension(avatar.FileName),
                    Extension = Path.GetExtension(avatar.FileName),

                    SizeInBytes = avatar.Length,
                    Mime = avatar.ContentType
                };

                var response = await mediator.Send(request);
                return Ok(response);
            }
            catch(InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<ActionResult<UserProfileInfo>> Edit(EditUserCommand request)
        {
            try
            {
                return Ok(await mediator.Send(request));
            }
            catch(InvalidOperationException e)
            {
                return NotFound();
            }
            catch(ValidationException e)
            {
                return BadRequest(e.Message);
            }
        }

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
