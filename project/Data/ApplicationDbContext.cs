using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using WebApplication6.Models;

namespace WebApplication6.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        { 

        }
        public DbSet<User> Users {  get; set; }
        public DbSet<Product> Products {  get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warning => 
            warning.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

    }
}
