using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistence;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // create a db
            var host = CreateHostBuilder(args).Build();
            // using: scope will be disposed of after Main() method is finished
            using var scope = host.Services.CreateScope();

            var services = scope.ServiceProvider;

            try
            {
                // DataContext comes from Persistence namespace
                var context = services.GetRequiredService<DataContext>();
                // MigrateAsync() will create the database if it does not exist
                await context.Database.MigrateAsync();
                // async seeder function SeedData() will populate the DB is there is no activities in it
                await Seed.SeedData(context);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occured during migration");
            }

            // Start the application
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // startup will check if there is a db, if not then create one
                    webBuilder.UseStartup<Startup>();
                });
    }
}
