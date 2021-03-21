using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    // IdentityDbContext is installed from Nuget Microsoft.AspNetCore.Identity.EntityFrameworkCore
    // once installed, run a migration from root folder "dotnet ef migrations add IdentityAdded -p Persistence -s API"
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        // DbSet represents a table in db
        public DbSet<Activity> Activities { get; set; }
    }
}