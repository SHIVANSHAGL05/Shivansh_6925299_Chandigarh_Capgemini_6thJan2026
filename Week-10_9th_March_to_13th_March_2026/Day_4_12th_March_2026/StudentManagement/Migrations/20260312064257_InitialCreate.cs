using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StudentManagement.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    StudentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnrollmentDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.StudentId);
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "StudentId", "Email", "EnrollmentDate", "FullName" },
                values: new object[,]
                {
                    { 1, "alice@university.edu", new DateTime(2023, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Alice Johnson" },
                    { 2, "bob@university.edu", new DateTime(2023, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bob Smith" },
                    { 3, "carol@university.edu", new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Carol Williams" },
                    { 4, "david@university.edu", new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "David Brown" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Students");
        }
    }
}
