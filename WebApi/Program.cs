using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Repositories;
using WebApi.Services;

namespace WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

        .
            builder.Services.AddControllers();

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

       
            builder.Services.AddScoped<IUserRepository, UserRepository>();

           
            builder.Services.AddScoped<IUserService, UserService>();

         
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowBlazor", policy =>
                {
                    policy.WithOrigins(
                            "https://localhost:7002",
                            "http://localhost:5002",
                            "http://localhost:5024",    
                            "https://localhost:7024")  
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials(); 
                });
            });

            builder.Services.AddOpenApi();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

           
            app.UseCors("AllowBlazor");

            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}