using CqrsDemo.Domain.Entities.Order;
using Microsoft.EntityFrameworkCore;

namespace CqrsDemo.Infrastructure.Persistence
{
    public class ReadDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }

        public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReadDbContext).Assembly);
        }
    }
}
