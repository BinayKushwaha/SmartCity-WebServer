using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SmartCity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class seedcommissionrates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "CommissionRates",
                columns: new[] { "Id", "CreatedAt", "IsActive", "Label", "MaxPrice", "MinPrice", "RatePercentage", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Below 50 Lakhs", 4999999m, 0m, 2.00m, null },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "50 Lakhs to 1 Crore", 10000000m, 5000000m, 1.75m, null },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Above 1 Crore", 9999999999m, 10000001m, 1.50m, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CommissionRates",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CommissionRates",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CommissionRates",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
