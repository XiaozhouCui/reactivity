using System.Text;
using API.Services;
using Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("super secret key"));

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
                });
            // service will be available when injected into account controller
            services.AddScoped<TokenService>();

            return services;
        }
    }
}