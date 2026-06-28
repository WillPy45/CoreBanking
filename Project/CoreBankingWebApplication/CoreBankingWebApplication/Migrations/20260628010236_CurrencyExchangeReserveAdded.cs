using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreBankingWebApplication.Migrations
{
    /// <inheritdoc />
    public partial class CurrencyExchangeReserveAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BalanceTransactions_Customers_CustomerId",
                table: "BalanceTransactions");

            migrationBuilder.DropIndex(
                name: "IX_BalanceTransactions_CustomerId",
                table: "BalanceTransactions");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "BalanceTransactions");

            migrationBuilder.CreateTable(
                name: "CurrencyExchangeReserves",
                columns: table => new
                {
                    CurrencyExchangeReserveId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(65,30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyExchangeReserves", x => x.CurrencyExchangeReserveId);
                    table.ForeignKey(
                        name: "FK_CurrencyExchangeReserves_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchangeReserves_CurrencyId",
                table: "CurrencyExchangeReserves",
                column: "CurrencyId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrencyExchangeReserves");

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "BalanceTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BalanceTransactions_CustomerId",
                table: "BalanceTransactions",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_BalanceTransactions_Customers_CustomerId",
                table: "BalanceTransactions",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId");
        }
    }
}
