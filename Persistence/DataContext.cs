using Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
  public class DataContext : DbContext
  {
    public DataContext(DbContextOptions options) : base(options)
    {
    }
    
    // DbSet represents a table in db
    public DbSet<Activity> Activities { get; set; }
  }
}