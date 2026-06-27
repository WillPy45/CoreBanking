using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreBankingWebApplication.Migrations
{
    /// <inheritdoc />
    public partial class AddBalanceTransactionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BalanceTransactions",
                columns: table => new
                {
                    BalanceTransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SenderBankAccountId = table.Column<int>(type: "int", nullable: false),
                    ReceiverBankAccountId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BalanceTransactions", x => x.BalanceTransactionId);
                    table.ForeignKey(
                        name: "FK_BalanceTransactions_BankAccounts_ReceiverBankAccountId",
                        column: x => x.ReceiverBankAccountId,
                        principalTable: "BankAccounts",
                        principalColumn: "BankAccountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BalanceTransactions_BankAccounts_SenderBankAccountId",
                        column: x => x.SenderBankAccountId,
                        principalTable: "BankAccounts",
                        principalColumn: "BankAccountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BalanceTransactions_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_BalanceTransactions_CustomerId",
                table: "BalanceTransactions",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_BalanceTransactions_ReceiverBankAccountId",
                table: "BalanceTransactions",
                column: "ReceiverBankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BalanceTransactions_SenderBankAccountId",
                table: "BalanceTransactions",
                column: "SenderBankAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BalanceTransactions");
        }
    }
}
