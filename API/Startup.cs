using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using API.Extensions;
using FluentValidation.AspNetCore;
using Application.Activities;
using API.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using API.SignalR;

namespace API
{
    public class Startup
    {
        private readonly IConfiguration _config;
        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // ConfigureServices is the Dependency Injection container, injectable services listed here
        public void ConfigureServices(IServiceCollection services)
        {
            // make the app aware of FluentValidation from NuGet
            services
                .AddControllers(opt =>
                {
                    // create authorisation policy: every endpoint in api requires authentication
                    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                    opt.Filters.Add(new AuthorizeFilter(policy));
                })
                .AddFluentValidation(config =>
                {
                    // any validators in Application layer will be registered with controllers
                    config.RegisterValidatorsFromAssemblyContaining<Create>();
                });
            // all methods are saved in API.Extensions
            services.AddApplicationServices(_config);
            // basic identity configuration
            services.AddIdentityServices(_config);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // It adds Middlewares to the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ExceptionMiddleware>(); // error handling middleware should be added first

            // security settings
            app.UseXContentTypeOptions();
            app.UseReferrerPolicy(opt => opt.NoReferrer()); // tell browser not to send referrer info
            app.UseXXssProtection(opt => opt.EnabledWithBlockMode()); // Cross-site-scripting protection
            app.UseXfo(opt => opt.Deny()); // prevent from being used in iframes somewhere else
            app.UseCspReportOnly(opt => opt // Content-Security-Policy-Report-Only header
                // chain various policies (report only)
                .BlockAllMixedContent() // https only
                .StyleSources(s => s.Self()) // css files only come from its own domain name
                .FontSources(s => s.Self())
                .FormActions(s => s.Self())
                .FrameAncestors(s => s.Self())
                .ImageSources(s => s.Self())
                .ScriptSources(s => s.Self())
            );

            if (env.IsDevelopment())
            {
                // order of middlewares is important
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
            }

            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseDefaultFiles();
            app.UseStaticFiles(); // default folder: wwwroot

            app.UseCors("CorsPolicy");

            // authentication must come BEFORE authorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // add API controller endpoints
                endpoints.MapControllers();
                // Instead of API controllers, we use SignalR Hubs as endpoints for client
                endpoints.MapHub<ChatHub>("/chat");
                // deal with frontend routes (react-router-dom), need a controller "Fallback" with action called "Index"
                endpoints.MapFallbackToController("Index", "Fallback");
            });
        }
    }
}
