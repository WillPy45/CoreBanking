using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreBankingWebApplication.Migrations
{
    /// <inheritdoc />
    public partial class CurrencyExchange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CurrencyExchange",
                columns: table => new
                {
                    CurrencyExchangeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SenderBankAccountId = table.Column<int>(type: "int", nullable: false),
                    CurrencyExchangeReserveId = table.Column<int>(type: "int", nullable: false),
                    ReceiverBankAccountId = table.Column<int>(type: "int", nullable: false),
                    SourceAmount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    GrossAmount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    BankRateInPercentage = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    BankDiscount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    NetAmount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyExchange", x => x.CurrencyExchangeId);
                    table.ForeignKey(
                        name: "FK_CurrencyExchange_BankAccounts_ReceiverBankAccountId",
                        column: x => x.ReceiverBankAccountId,
                        principalTable: "BankAccounts",
                        principalColumn: "BankAccountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CurrencyExchange_BankAccounts_SenderBankAccountId",
                        column: x => x.SenderBankAccountId,
                        principalTable: "BankAccounts",
                        principalColumn: "BankAccountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CurrencyExchange_CurrencyExchangeReserves_CurrencyExchangeRe~",
                        column: x => x.CurrencyExchangeReserveId,
                        principalTable: "CurrencyExchangeReserves",
                        principalColumn: "CurrencyExchangeReserveId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchange_CurrencyExchangeReserveId",
                table: "CurrencyExchange",
                column: "CurrencyExchangeReserveId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchange_ReceiverBankAccountId",
                table: "CurrencyExchange",
                column: "ReceiverBankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchange_SenderBankAccountId",
                table: "CurrencyExchange",
                column: "SenderBankAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrencyExchange");
        }
    }
}
