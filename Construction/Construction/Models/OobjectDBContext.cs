using Microsoft.EntityFrameworkCore; 
namespace Construction.Models
{
    public class OobjectDBContext : DbContext
    {
        public OobjectDBContext(DbContextOptions<OobjectDBContext> options) : base(options)
        {
        }
        public DbSet<Oobject> Oobjects { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Foremen> Foremens { get; set; }
        public DbSet<Worker> Workers { get; set; }
    }
}
