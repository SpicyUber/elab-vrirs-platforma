using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Application.Services
{
    public class JwtService
    {
        private readonly UserManager<User> userManager;
        private readonly IConfiguration config;
        public JwtService(UserManager<User> userManager, IConfiguration config) { this.userManager = userManager; this.config = config;  }
        public async Task<string> GenerateToken(User user)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
            new Claim(ClaimTypes.Email,user.Email), new Claim(ClaimTypes.Role, (await userManager.GetRolesAsync(user))[0])

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("JwtSettings")["SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: config.GetSection("JwtSettings")["Issuer"],
                audience: config.GetSection("JwtSettings")["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(config.GetSection("JwtSettings")["ExpiryMinutes"])),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
