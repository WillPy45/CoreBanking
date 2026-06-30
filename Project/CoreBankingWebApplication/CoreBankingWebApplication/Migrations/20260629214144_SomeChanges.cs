using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreBankingWebApplication.Migrations
{
    /// <inheritdoc />
    public partial class SomeChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "CurrencyExchanges",
                newName: "BankUnitPrice");

            migrationBuilder.RenameColumn(
                name: "BankRateInPercentage",
                table: "CurrencyExchanges",
                newName: "BankDiscountInPercentage");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BankUnitPrice",
                table: "CurrencyExchanges",
                newName: "UnitPrice");

            migrationBuilder.RenameColumn(
                name: "BankDiscountInPercentage",
                table: "CurrencyExchanges",
                newName: "BankRateInPercentage");
        }
    }
}
