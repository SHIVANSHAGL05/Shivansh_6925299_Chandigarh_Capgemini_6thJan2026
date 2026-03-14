using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LibraryManagement.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    BookId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Author = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ISBN = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Genre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PublishedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.BookId);
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "BookId", "Author", "Description", "Genre", "ISBN", "IsAvailable", "Price", "PublishedDate", "Title" },
                values: new object[,]
                {
                    { 1, "Robert C. Martin", "A handbook of agile software craftsmanship.", "Programming", "978-0132350884", true, 35.99m, new DateTime(2008, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Clean Code" },
                    { 2, "Andrew Hunt", "Your journey to mastery.", "Programming", "978-0135957059", true, 49.99m, new DateTime(2019, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "The Pragmatic Programmer" },
                    { 3, "Gang of Four", "Elements of reusable object-oriented software.", "Software Architecture", "978-0201633610", false, 54.99m, new DateTime(1994, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Design Patterns" },
                    { 4, "Jon Skeet", "A deep dive into the C# language.", "Programming", "978-1617294532", true, 44.99m, new DateTime(2019, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "C# in Depth" },
                    { 5, "Andrew Lock", "Build cross-platform web apps with ASP.NET Core.", "Web Development", "978-1617298301", true, 59.99m, new DateTime(2021, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ASP.NET Core in Action" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Books");
        }
    }
}
