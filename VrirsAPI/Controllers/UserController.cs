using Application.Commands.User;
using Application.DTOs.User;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace VrirsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator mediator;

        public UserController(IMediator mediator)
        {
            this.mediator = mediator;
            
        }
        

        [HttpPost("register")]
        public async Task<ActionResult<UserSessionInfo>> Register([FromBody] RegisterCommand request)
        {
            try
            {
                var response = await mediator.Send(request);
                return Ok(response);
            }catch(InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserSessionInfo>> Register([FromBody] LoginCommand request)
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
    }
}
