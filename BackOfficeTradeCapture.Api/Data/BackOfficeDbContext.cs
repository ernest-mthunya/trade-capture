using BackOfficeTradeCapture.Api.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BackOfficeTradeCapture.Api.Data
{
    public class BackOfficeDbContext : DbContext
    {
        public DbSet<TradeEntity> Trades { get; set; } = default!;
        public BackOfficeDbContext(DbContextOptions<BackOfficeDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }


    }
}
