using Application;
using Application.Services;
using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Persistence.UnitOfWork.Implementation;
using Infrastructure.Persistence.UnitOfWork.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Numerics;
using System.Text;
using System.Text.Json.Serialization;

namespace VrirsAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            AddServices(builder);
            SetupMediatR(builder);
            SetupJWTAuth(builder);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();
            app.UseAuthentication(); 
            app.UseAuthorization();
            app.Run();
        }

        private static void SetupJWTAuth(WebApplicationBuilder builder)
        {
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"]!;

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            });
        }

        private static void AddServices(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers().AddJsonOptions(opt => { opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); }); ;

            AddDependencyInjections(builder);

            builder.Services.AddIdentity<User, IdentityRole<Guid>>(options => { options.Password.RequiredLength = 6; options.User.RequireUniqueEmail = true; options.SignIn.RequireConfirmedEmail = false; }).AddEntityFrameworkStores<VrirsDbContext>();
           
            builder.Services.AddDbContext<VrirsDbContext>((options) => { options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")); });
            
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
        }

        

        private static void SetupMediatR(WebApplicationBuilder builder)=> builder.Services.AddMediatR(cfg => { cfg.LicenseKey = builder.Configuration.GetSection("MediatR")["Key"]; cfg.RegisterServicesFromAssembly(typeof(MediatRHook).Assembly); });


        private static void AddDependencyInjections(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            builder.Services.AddScoped<UserManager<User>>();
            builder.Services.AddScoped<JwtService>();
        }
    }


}
