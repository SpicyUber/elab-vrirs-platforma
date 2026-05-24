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
        private readonly UserManager<Domain.Entities.User> userManager;
        private readonly IConfiguration config;

        public LoginCommandHandler(UserManager<Domain.Entities.User> userManager, IConfiguration config)
        {
            this.userManager = userManager;
            this.config = config;
        }

        public async Task<UserSessionInfo> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null || !user.IsActive)
                throw new UnauthorizedAccessException("Invalid credentials.");

            var passwordValid = await userManager.CheckPasswordAsync(user, request.Password);
            if (!passwordValid)
                throw new UnauthorizedAccessException("Invalid credentials.");

            var roles = await userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "Student";

            var token = await new JwtService(userManager, config).GenerateToken(user);

            return new UserSessionInfo(user, role, token);
        }
    }
}
