using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreBankingWebApplication.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_Customers_CustomerId1",
                table: "BankAccounts");

            migrationBuilder.DropIndex(
                name: "IX_BankAccounts_CustomerId1",
                table: "BankAccounts");

            migrationBuilder.DropColumn(
                name: "CustomerId1",
                table: "BankAccounts");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "BankAccounts",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_CustomerId",
                table: "BankAccounts",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_Customers_CustomerId",
                table: "BankAccounts",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankAccounts_Customers_CustomerId",
                table: "BankAccounts");

            migrationBuilder.DropIndex(
                name: "IX_BankAccounts_CustomerId",
                table: "BankAccounts");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerId",
                table: "BankAccounts",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "CustomerId1",
                table: "BankAccounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_CustomerId1",
                table: "BankAccounts",
                column: "CustomerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_BankAccounts_Customers_CustomerId1",
                table: "BankAccounts",
                column: "CustomerId1",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
