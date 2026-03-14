using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackOfficeTradeCapture.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedIndeces : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Trades_Account_Symbol",
                table: "Trades",
                columns: new[] { "Account", "Symbol" });

            migrationBuilder.CreateIndex(
                name: "IX_Trades_TradeTime",
                table: "Trades",
                column: "TradeTime");

            migrationBuilder.CreateIndex(
                name: "IX_Trades_TradeTime_Account_Symbol",
                table: "Trades",
                columns: new[] { "TradeTime", "Account", "Symbol" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Trades_Account_Symbol",
                table: "Trades");

            migrationBuilder.DropIndex(
                name: "IX_Trades_TradeTime",
                table: "Trades");

            migrationBuilder.DropIndex(
                name: "IX_Trades_TradeTime_Account_Symbol",
                table: "Trades");
        }
    }
}
