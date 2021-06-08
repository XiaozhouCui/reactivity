using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    // IdentityDbContext is installed from Nuget Microsoft.AspNetCore.Identity.EntityFrameworkCore
    // once installed, run a migration from root folder "dotnet ef migrations add IdentityAdded -p Persistence -s API"
    public class DataContext : IdentityDbContext<AppUser>
    {
        // "base" is for parent class
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        // DbSet represents a table in db: Activities table
        public DbSet<Activity> Activities { get; set; }

        // To add many-to-many relationship, add a join table named ActivityAttendees
        public DbSet<ActivityAttendee> ActivityAttendees { get; set; }

        // To query the photo collection directly, add a table named Photos
        public DbSet<Photo> Photos { get; set; }

        // override the OnModelCreating method, to add additional configurations
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // builder will grant acces to entity configurations
            base.OnModelCreating(builder);
            // set primary key for the ActivityAttendees table
            builder.Entity<ActivityAttendee>(x => x.HasKey(aa => new { aa.AppUserId, aa.ActivityId }));
            // set one side of the relationship
            builder.Entity<ActivityAttendee>()
                .HasOne(u => u.AppUser)
                .WithMany(a => a.Activities)
                .HasForeignKey(aa => aa.AppUserId);
            // set the other side of the relationship
            builder.Entity<ActivityAttendee>()
                .HasOne(u => u.Activity)
                .WithMany(a => a.Attendees)
                .HasForeignKey(aa => aa.ActivityId);
        }
    }
}