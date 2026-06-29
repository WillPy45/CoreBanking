using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreBankingWebApplication.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyExchange_BankAccounts_ReceiverBankAccountId",
                table: "CurrencyExchange");

            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyExchange_BankAccounts_SenderBankAccountId",
                table: "CurrencyExchange");

            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyExchange_CurrencyExchangeReserves_CurrencyExchangeRe~",
                table: "CurrencyExchange");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CurrencyExchange",
                table: "CurrencyExchange");

            migrationBuilder.RenameTable(
                name: "CurrencyExchange",
                newName: "CurrencyExchanges");

            migrationBuilder.RenameIndex(
                name: "IX_CurrencyExchange_SenderBankAccountId",
                table: "CurrencyExchanges",
                newName: "IX_CurrencyExchanges_SenderBankAccountId");

            migrationBuilder.RenameIndex(
                name: "IX_CurrencyExchange_ReceiverBankAccountId",
                table: "CurrencyExchanges",
                newName: "IX_CurrencyExchanges_ReceiverBankAccountId");

            migrationBuilder.RenameIndex(
                name: "IX_CurrencyExchange_CurrencyExchangeReserveId",
                table: "CurrencyExchanges",
                newName: "IX_CurrencyExchanges_CurrencyExchangeReserveId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CurrencyExchanges",
                table: "CurrencyExchanges",
                column: "CurrencyExchangeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyExchanges_BankAccounts_ReceiverBankAccountId",
                table: "CurrencyExchanges",
                column: "ReceiverBankAccountId",
                principalTable: "BankAccounts",
                principalColumn: "BankAccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyExchanges_BankAccounts_SenderBankAccountId",
                table: "CurrencyExchanges",
                column: "SenderBankAccountId",
                principalTable: "BankAccounts",
                principalColumn: "BankAccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyExchanges_CurrencyExchangeReserves_CurrencyExchangeR~",
                table: "CurrencyExchanges",
                column: "CurrencyExchangeReserveId",
                principalTable: "CurrencyExchangeReserves",
                principalColumn: "CurrencyExchangeReserveId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyExchanges_BankAccounts_ReceiverBankAccountId",
                table: "CurrencyExchanges");

            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyExchanges_BankAccounts_SenderBankAccountId",
                table: "CurrencyExchanges");

            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyExchanges_CurrencyExchangeReserves_CurrencyExchangeR~",
                table: "CurrencyExchanges");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CurrencyExchanges",
                table: "CurrencyExchanges");

            migrationBuilder.RenameTable(
                name: "CurrencyExchanges",
                newName: "CurrencyExchange");

            migrationBuilder.RenameIndex(
                name: "IX_CurrencyExchanges_SenderBankAccountId",
                table: "CurrencyExchange",
                newName: "IX_CurrencyExchange_SenderBankAccountId");

            migrationBuilder.RenameIndex(
                name: "IX_CurrencyExchanges_ReceiverBankAccountId",
                table: "CurrencyExchange",
                newName: "IX_CurrencyExchange_ReceiverBankAccountId");

            migrationBuilder.RenameIndex(
                name: "IX_CurrencyExchanges_CurrencyExchangeReserveId",
                table: "CurrencyExchange",
                newName: "IX_CurrencyExchange_CurrencyExchangeReserveId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CurrencyExchange",
                table: "CurrencyExchange",
                column: "CurrencyExchangeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyExchange_BankAccounts_ReceiverBankAccountId",
                table: "CurrencyExchange",
                column: "ReceiverBankAccountId",
                principalTable: "BankAccounts",
                principalColumn: "BankAccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyExchange_BankAccounts_SenderBankAccountId",
                table: "CurrencyExchange",
                column: "SenderBankAccountId",
                principalTable: "BankAccounts",
                principalColumn: "BankAccountId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyExchange_CurrencyExchangeReserves_CurrencyExchangeRe~",
                table: "CurrencyExchange",
                column: "CurrencyExchangeReserveId",
                principalTable: "CurrencyExchangeReserves",
                principalColumn: "CurrencyExchangeReserveId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
