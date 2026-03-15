using BackOfficeTradeCapture.Api.Entities;
using BackOfficeTradeCapture.Api.Models;
using BackOfficeTradeCapture.Api.Models.BackOfficeTradeCapture.Api.Models;
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
            modelBuilder.Entity<ReportRow>(e =>
            {
                e.HasNoKey().ToView(null);
                e.Property(r => r.Account).HasColumnName("Account");
                e.Property(r => r.Symbol).HasColumnName("Symbol");
                e.Property(r => r.TotalQty).HasColumnName("TotalQty");
                e.Property(r => r.AvgPrice).HasColumnName("AvgPrice").HasPrecision(18, 6);
                e.Property(r => r.NotionalBase).HasColumnName("NotionalBase").HasPrecision(18, 2);
                e.Property(r => r.BaseCcy).HasColumnName("BaseCcy");
            });

            return modelBuilder;
        }
    }
}