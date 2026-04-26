using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartHealthcare.API.Migrations
{
    /// <inheritdoc />
    public partial class RealtimeBillPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAt",
                table: "Bills",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransactionReference",
                table: "Bills",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 4, 6, 5, 18, 9, 638, DateTimeKind.Utc).AddTicks(9486), "$2a$11$ooMBwTCh/I9RYNJFb60oSOB2a0VFedGceFJSjDLMgvsbXp0OI6M1." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaidAt",
                table: "Bills");

            migrationBuilder.DropColumn(
                name: "TransactionReference",
                table: "Bills");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 4, 6, 4, 50, 25, 718, DateTimeKind.Utc).AddTicks(7945), "$2a$11$YJ3zSDJXvt8W/2aikInDGe1qT8Bf2qXaItCPtAJkS3M/g.Y/Vm10K" });
        }
    }
}
