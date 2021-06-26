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
using System;

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

            // services.AddDbContext<DataContext>(opt =>
            // {
            //     // opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
            //     opt.UseNpgsql(config.GetConnectionString("DefaultConnection"));
            // });

            // read DB connection string from Heroku env
            services.AddDbContext<DataContext>(options =>
            {
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                string connStr;

                // Depending on if in development or production, use either Heroku-provided
                // connection string, or development connection string from env var.
                if (env == "Development")
                {
                    // Use connection string from file.
                    connStr = config.GetConnectionString("DefaultConnection");
                }
                else
                {
                    // Use connection string provided at runtime by Heroku.
                    var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

                    // Parse connection URL to connection string for Npgsql
                    connUrl = connUrl.Replace("postgres://", string.Empty);
                    var pgUserPass = connUrl.Split("@")[0];
                    var pgHostPortDb = connUrl.Split("@")[1];
                    var pgHostPort = pgHostPortDb.Split("/")[0];
                    var pgDb = pgHostPortDb.Split("/")[1];
                    var pgUser = pgUserPass.Split(":")[0];
                    var pgPass = pgUserPass.Split(":")[1];
                    var pgHost = pgHostPort.Split(":")[0];
                    var pgPort = pgHostPort.Split(":")[1];

                    connStr = $"Server={pgHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb}; SSL Mode=Require; Trust Server Certificate=true";
                }

                // Whether the connection string came from the local development configuration file
                // or from the environment variable from Heroku, use it to set up your DbContext.
                options.UseNpgsql(connStr);
            });


            // add CORS header
            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials() // allow connection to SignalR hub
                        .WithOrigins("http://localhost:3000");
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
            // add SignalR as a service, to handle comments in real-time
            services.AddSignalR();

            return services;
        }
    }
}