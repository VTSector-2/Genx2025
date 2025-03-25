using Hackathon.Models;
using Microsoft.EntityFrameworkCore;

namespace Hackathon.DB
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Define DbSets for your tables
        //public DbSet<ErrorViewModel> YourEntities { get; set; }
        public DbSet<Site> Mast_Site { get; set; }

        public DbSet<Risk> Risk { get; set; }
    }
}
