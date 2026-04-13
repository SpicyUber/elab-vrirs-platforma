using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.UnitOfWork.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace VrirsAPI.Controllers
{
    [ApiController]
    [Route("api/")]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork uow;
        private readonly UserManager<User> userManager;

        public UserController(IUnitOfWork uow, UserManager<User> userManager)
        {
            this.uow = uow;
            this.userManager = userManager;
        }
        public class RegisterDto
        {
            [Required]
            public string FullName { get; set; } = string.Empty;

            [Required, EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required, MinLength(8)]
            public string Password { get; set; } = string.Empty;

 
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return Conflict("A user with this email already exists.");

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                UserName = dto.Email, // Identity still needs UserName internally
                IsActive = true,
            };

            var result = await userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Assign default role
            await userManager.AddToRoleAsync(user, "Student");

            return Ok("User registered successfully.");
        }
    }
}
