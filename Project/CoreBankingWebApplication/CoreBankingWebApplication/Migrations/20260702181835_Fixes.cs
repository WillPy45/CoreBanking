using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreBankingWebApplication.Migrations
{
    /// <inheritdoc />
    public partial class Fixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyExchanges_CurrencyExchangeReserves_CurrencyExchangeR~",
                table: "CurrencyExchanges");

            migrationBuilder.RenameColumn(
                name: "CurrencyExchangeReserveId",
                table: "CurrencyExchanges",
                newName: "DebitedExchangeReserveId");

            migrationBuilder.RenameIndex(
                name: "IX_CurrencyExchanges_CurrencyExchangeReserveId",
                table: "CurrencyExchanges",
                newName: "IX_CurrencyExchanges_DebitedExchangeReserveId");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Customers",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Customers",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Customers",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateAt",
                table: "Customers",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "AccreditedExchangeReserveId",
                table: "CurrencyExchanges",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateAt",
                table: "BankAccounts",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchanges_AccreditedExchangeReserveId",
                table: "CurrencyExchanges",
                column: "AccreditedExchangeReserveId");

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyExchanges_CurrencyExchangeReserves_AccreditedExchang~",
                table: "CurrencyExchanges",
                column: "AccreditedExchangeReserveId",
                principalTable: "CurrencyExchangeReserves",
                principalColumn: "CurrencyExchangeReserveId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyExchanges_CurrencyExchangeReserves_DebitedExchangeRe~",
                table: "CurrencyExchanges",
                column: "DebitedExchangeReserveId",
                principalTable: "CurrencyExchangeReserves",
                principalColumn: "CurrencyExchangeReserveId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyExchanges_CurrencyExchangeReserves_AccreditedExchang~",
                table: "CurrencyExchanges");

            migrationBuilder.DropForeignKey(
                name: "FK_CurrencyExchanges_CurrencyExchangeReserves_DebitedExchangeRe~",
                table: "CurrencyExchanges");

            migrationBuilder.DropIndex(
                name: "IX_CurrencyExchanges_AccreditedExchangeReserveId",
                table: "CurrencyExchanges");

            migrationBuilder.DropColumn(
                name: "CreateAt",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "AccreditedExchangeReserveId",
                table: "CurrencyExchanges");

            migrationBuilder.DropColumn(
                name: "CreateAt",
                table: "BankAccounts");

            migrationBuilder.RenameColumn(
                name: "DebitedExchangeReserveId",
                table: "CurrencyExchanges",
                newName: "CurrencyExchangeReserveId");

            migrationBuilder.RenameIndex(
                name: "IX_CurrencyExchanges_DebitedExchangeReserveId",
                table: "CurrencyExchanges",
                newName: "IX_CurrencyExchanges_CurrencyExchangeReserveId");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "LastName",
                keyValue: null,
                column: "LastName",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Customers",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "FirstName",
                keyValue: null,
                column: "FirstName",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Customers",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Address",
                keyValue: null,
                column: "Address",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Customers",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_CurrencyExchanges_CurrencyExchangeReserves_CurrencyExchangeR~",
                table: "CurrencyExchanges",
                column: "CurrencyExchangeReserveId",
                principalTable: "CurrencyExchangeReserves",
                principalColumn: "CurrencyExchangeReserveId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
