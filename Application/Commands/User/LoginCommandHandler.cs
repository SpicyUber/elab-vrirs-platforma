using Application.DTOs.User;
using Application.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.User
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, UserSessionInfo>
    {
        private readonly UserManager<Domain.Entities.User> _userManager;
        private readonly IConfiguration _config;

        public LoginCommandHandler(UserManager<Domain.Entities.User> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        public async Task<UserSessionInfo> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !user.IsActive)
                throw new UnauthorizedAccessException("Invalid credentials.");

            var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!passwordValid)
                throw new UnauthorizedAccessException("Invalid credentials.");

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "Student";

            var token = await new JwtService(_userManager, _config).GenerateToken(user);

            return new UserSessionInfo(user, role, token);
        }
    }
}
