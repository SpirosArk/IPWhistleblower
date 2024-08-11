using IPWhistleblower.Entities;
using Microsoft.EntityFrameworkCore;

namespace IPWhistleblower
{
    public class ApplicationDbContext : DbContext 
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Country> Countries { get; set; }
        public DbSet<IPAddress> IPAddresses { get; set; }
    }
}
