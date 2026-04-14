using Application.DTOs.User;
using Application.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.User
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, UserSessionInfo>
    {
        
              private readonly UserManager<Domain.Entities.User> userManager;
              private readonly IConfiguration config;
        

        public RegisterCommandHandler(UserManager<Domain.Entities.User> userManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.config = configuration;
            
        }

        public async Task<UserSessionInfo> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                throw new InvalidOperationException("A user with this email already exists.");

            var user = new Domain.Entities.User
            {
                FullName = request.FullName,
                Email = request.Email,
                UserName = request.Email,
                IsActive = true,
            };

            var result = await userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));

            await userManager.AddToRoleAsync(user, "Student");

            string token = await new JwtService(userManager,config).GenerateToken(user);

            return new UserSessionInfo(user, "Student", token);
        }

      
    
    }
}
