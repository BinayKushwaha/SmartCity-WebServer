using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartCity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addcommissionratestableandupdateretailproperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRoles",
                table: "UserRoles");

            migrationBuilder.RenameTable(
                name: "UserRoles",
                newName: "IdentityUserRole");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Properties",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AddColumn<string>(
                name: "BrokerId",
                table: "Properties",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionAmount",
                table: "Properties",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "CommissionRateId",
                table: "Properties",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Properties",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Properties",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_IdentityUserRole",
                table: "IdentityUserRole",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.CreateTable(
                name: "CommissionRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Label = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MinPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RatePercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommissionRates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Properties_BrokerId",
                table: "Properties",
                column: "BrokerId");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_CommissionRateId",
                table: "Properties",
                column: "CommissionRateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_CommissionRates_CommissionRateId",
                table: "Properties",
                column: "CommissionRateId",
                principalTable: "CommissionRates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Users_BrokerId",
                table: "Properties",
                column: "BrokerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Properties_CommissionRates_CommissionRateId",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Users_BrokerId",
                table: "Properties");

            migrationBuilder.DropTable(
                name: "CommissionRates");

            migrationBuilder.DropIndex(
                name: "IX_Properties_BrokerId",
                table: "Properties");

            migrationBuilder.DropIndex(
                name: "IX_Properties_CommissionRateId",
                table: "Properties");

            migrationBuilder.DropPrimaryKey(
                name: "PK_IdentityUserRole",
                table: "IdentityUserRole");

            migrationBuilder.DropColumn(
                name: "BrokerId",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "CommissionAmount",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "CommissionRateId",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Properties");

            migrationBuilder.RenameTable(
                name: "IdentityUserRole",
                newName: "UserRoles");

            migrationBuilder.AlterColumn<float>(
                name: "Price",
                table: "Properties",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRoles",
                table: "UserRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                });
        }
    }
}
