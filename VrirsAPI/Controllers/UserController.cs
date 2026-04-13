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
           
            return Ok("Not implemented.");
        }
    }
}
