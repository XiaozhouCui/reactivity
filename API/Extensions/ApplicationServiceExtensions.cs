using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using AutoMapper;
using Persistence;
using Application.Core;
using Application.Activities;
using Application.Interfaces;
using Infrastructure.Security;
using Infrastructure.Photos;

namespace API.Extensions
{
    // move methods from Startup.cs here, refactoring the startup processes
    public static class ApplicationServiceExtensions
    {
        // extending IServiceCollection
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            });
            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseSqlite(config.GetConnectionString("DefaultConnection"));

            });
            // add CORS header
            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyMethod().AllowAnyHeader().WithOrigins("http://localhost:3000");
                });
            });
            // add Mediator
            services.AddMediatR(typeof(List.Handler).Assembly); // where to find mediator handler
            // add AutoMapper
            services.AddAutoMapper(typeof(MappingProfiles).Assembly);
            // Added UserAccessor class from Infrastructure as a service
            // then we can get the logged in user's username from anywhere in the application
            services.AddScoped<IUserAccessor, UserAccessor>();
            // add PhotoAccessor as a service to interact with Cloudinary
            services.AddScoped<IPhotoAccessor, PhotoAccessor>();
            // configure Cloudinary, referring to the appsettings.json (secret file)
            services.Configure<CloudinarySettings>(config.GetSection("Cloudinary"));

            return services;
        }
    }
}