using Application.Commands.User;
using Application.DTOs.CourseEnrollment;
using Application.DTOs.User;
using Application.Queries.CourseEnrollment;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly int avatarSizeLimitInBytes = 524288;

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

        [Authorize]
        [HttpPost("avatar-upload")]
        public async Task<ActionResult<UserProfileInfo>> UploadAvatar(IFormFile avatar)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            try
            {
                ValidateAvatarUpload(avatar);

                using Stream uploadStream = avatar.OpenReadStream();

                UploadAvatarCommand request = new() { AvatarUploadStream = uploadStream, UserId = userId };

                request.Name = Path.GetFileNameWithoutExtension(avatar.FileName);
                request.Extension = Path.GetExtension(avatar.FileName);

                request.SizeInBytes = avatar.Length;
                request.Mime = avatar.ContentType;

                var response = await mediator.Send(request);

                return Ok(response);
            }
            catch (InvalidOperationException e)
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
            catch (InvalidOperationException e)
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
            catch (UnauthorizedAccessException e)
            {
                return BadRequest(e.Message);
            }
        }

        private void ValidateAvatarUpload(IFormFile avatarUpload)
        {
            if (avatarUpload.ContentType != "image/jpeg" && avatarUpload.ContentType != "image/png")
                throw new InvalidOperationException("Avatar must be in jpeg or png format.");
            if (avatarUpload.Length > avatarSizeLimitInBytes)
                throw new InvalidOperationException($"Avatar must be under {avatarSizeLimitInBytes / 1024} KB.");
            if (avatarUpload.FileName.Length == 0)
                throw new InvalidOperationException("File name cannot be empty.");
        }
    }
}
