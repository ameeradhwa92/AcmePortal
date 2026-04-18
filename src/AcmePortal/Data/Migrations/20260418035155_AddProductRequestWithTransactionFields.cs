using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AcmePortal.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProductRequestWithTransactionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "ProductRequests",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "ProductRequests",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfPurchased",
                table: "ProductRequests",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "ProductRequests",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "ProductRequests",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "TransactionRefNo",
                table: "ProductRequests",
                type: "TEXT",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "ProductRequests");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "ProductRequests");

            migrationBuilder.DropColumn(
                name: "DateOfPurchased",
                table: "ProductRequests");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "ProductRequests");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "ProductRequests");

            migrationBuilder.DropColumn(
                name: "TransactionRefNo",
                table: "ProductRequests");
        }
    }
}
