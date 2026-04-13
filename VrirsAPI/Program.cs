using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Persistence.UnitOfWork.Implementation;
using Infrastructure.Persistence.UnitOfWork.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Numerics;

namespace VrirsAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            builder.Services.AddScoped<UserManager<User>>();
            builder.Services.AddIdentity<User, IdentityRole<Guid>>(options => { options.Password.RequiredLength = 6; options.User.RequireUniqueEmail = true; options.SignIn.RequireConfirmedEmail = false; }).AddEntityFrameworkStores<VrirsDbContext>();
            builder.Services.AddDbContext<VrirsDbContext>((options) => { options.UseSqlServer("Data Source=KABYLAKE;Initial Catalog=vrirs;Integrated Security=True;Trust Server Certificate=True"); });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

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

            app.Run();
        }
    }
}
