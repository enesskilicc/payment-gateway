using Microsoft.EntityFrameworkCore;
using PSP.DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PSP.INFRASTRUCTURE.Persistence
{
    public class PspDbContext : DbContext
    {
        public PspDbContext(DbContextOptions<PspDbContext> options) : base(options)
        {
          
        }

        public DbSet<Merchant> Merchants { get; set; }
        public DbSet<ApiKey> ApiKeys { get; set; }
        public DbSet<Payment> Payments => Set<Payment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PspDbContext).Assembly);
        }
    }
}
