using System.Text;
using System.Threading.Tasks;
using API.Services;
using Domain;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Persistence;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        // AddIdentityServices: configure Identity to be added in Startup class
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddIdentityCore<AppUser>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<DataContext>()
            .AddSignInManager<SignInManager<AppUser>>();

            // get JWT secret key from "appsettings.Development.json" (similar to .env)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));

            // use JwtBearer from Nuget
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    // validate if the token is valid
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        // validate the secret key stored on server ("super secret key")
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                    // SignalR uses web socket and doesn't have have an HTTP auth header
                    opt.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            // SignalR on client side will pass the jwt in query string "access_token"
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            // "/chat" is the endpoint for SignalR Hub
                            if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/chat")))
                            {
                                // store token into context
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            // only the host can modify activity details (to be added in controller as middleware before HttpPut)
            services.AddAuthorization(opt =>
            {
                // add auth policy "IsActivityHost"
                opt.AddPolicy("IsActivityHost", policy =>
                {
                    // add the newly created class IsHostRequirement as requirement
                    policy.Requirements.Add(new IsHostRequirement());
                });
            });

            // only need this to last as long as the method is running
            services.AddTransient<IAuthorizationHandler, IsHostRequirementHandler>();

            // service will be available when injected into account controller
            services.AddScoped<TokenService>();

            return services;
        }
    }
}