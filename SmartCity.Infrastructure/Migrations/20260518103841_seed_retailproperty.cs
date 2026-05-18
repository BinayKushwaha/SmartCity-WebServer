using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SmartCity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class seed_retailproperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Properties",
                columns: new[] { "Id", "BrokerId", "CommissionAmount", "CommissionRateId", "CreatedAt", "Features", "Location", "Price", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "a4392244-a556-4af8-94e9-e80725cbc880", 90000.00m, 1, new DateTime(2026, 1, 15, 10, 0, 0, 0, DateTimeKind.Utc), "Cozy 2BHK apartment with city views and 24/7 water supply.", "Baneshwor, Kathmandu", 4500000m, "Residential", null },
                    { 2, "a4392244-a556-4af8-94e9-e80725cbc880", 1275000.00m, 3, new DateTime(2026, 2, 1, 14, 30, 0, 0, DateTimeKind.Utc), "Premium office space on the ground floor with high foot traffic.", "Durbar Marg, Kathmandu", 85000000m, "Commercial", null },
                    { 3, "a4392244-a556-4af8-94e9-e80725cbc880", 2250000.00m, 3, new DateTime(2026, 3, 10, 9, 15, 0, 0, DateTimeKind.Utc), "Large manufacturing warehouse equipped with a 3-phase power supply.", "Patandhoka, Lalitpur", 150000000m, "Industrial", null },
                    { 4, "a4392244-a556-4af8-94e9-e80725cbc880", 70000.00m, 1, new DateTime(2026, 4, 5, 11, 0, 0, 0, DateTimeKind.Utc), "Highly fertile multi-crop farming land spanning 5 Ropanis with access road.", "Jhapa, Nepal", 3500000m, "Agricultural", null },
                    { 5, "a4392244-a556-4af8-94e9-e80725cbc880", 131250.00m, 2, new DateTime(2026, 4, 22, 16, 45, 0, 0, DateTimeKind.Utc), "Modern modular flat layout close to prime hub restaurant spaces.", "Jhamsikhel, Lalitpur", 7500000m, "Residential", null },
                    { 6, "a4392244-a556-4af8-94e9-e80725cbc880", 157500.00m, 2, new DateTime(2026, 5, 2, 13, 20, 0, 0, DateTimeKind.Utc), "Multi-story retail showroom block located in a prime commercial intersection.", "New Road, Pokhara", 9000000m, "Commercial", null },
                    { 7, "a4392244-a556-4af8-94e9-e80725cbc880", 277500.00m, 3, new DateTime(2026, 5, 14, 10, 0, 0, 0, DateTimeKind.Utc), "Semi-furnished family house featuring parking spaces for 1 car and 3 motorbikes.", "Chabahil, Kathmandu", 18500000m, "Residential", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 7);
        }
    }
}
