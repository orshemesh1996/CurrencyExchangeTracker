using Microsoft.EntityFrameworkCore;
using CurrencyExchangeTracker.Core.Models;

namespace CurrencyExchangeTracker.Infrastructure.Database
{
    public class CurrencyDbContext : DbContext
    {
        public CurrencyDbContext(DbContextOptions<CurrencyDbContext> options) : base(options)
        {
        }

        public DbSet<ExchangeRate> ExchangeRates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ExchangeRate>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.CurrencyPair).IsUnique();
                entity.Property(e => e.CurrentRate).HasPrecision(18, 8);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}