using BackOfficeTradeCapture.Api.Entities;
using BackOfficeTradeCapture.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BackOfficeTradeCapture.Api
{
    public static class ModelBuilderExtensions
    {
        public static ModelBuilder ConfigureTradeEntity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TradeEntity>(e =>
            {
                e.HasIndex(t => t.ExternalId)
                 .IsUnique()
                 .HasDatabaseName("UX_Trades_ExternalId");

                e.HasIndex(t => t.TradeTime)
                 .HasDatabaseName("IX_Trades_TradeTime");

                e.HasIndex(t => new { t.Account, t.Symbol })
                 .HasDatabaseName("IX_Trades_Account_Symbol");

                e.HasIndex(t => new { t.TradeTime, t.Account, t.Symbol })
                 .HasDatabaseName("IX_Trades_TradeTime_Account_Symbol");
            });

            return modelBuilder;
        }

        public static ModelBuilder ConfigureReportRow(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReportRow>()
                .HasNoKey()
                .ToView(null);

            return modelBuilder;
        }
    }
}
