using CqrsDemo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CqrsDemo.Infrastructure.Persistence
{
    public class WriteDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }

        public WriteDbContext(DbContextOptions<WriteDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(WriteDbContext).Assembly);
        }
    }
}
