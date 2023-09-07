using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace JwtWepApiCore.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "560c355f-a929-46d5-8885-ab0e9691ce72", "2", "User", "User" },
                    { "b9e8dc46-3a0a-4b60-af83-fd398a7687ac", "1", "Admin", "Admin" },
                    { "ef77134b-a2e3-4aba-88db-b55e7329f834", "3", "Hr", "HR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "560c355f-a929-46d5-8885-ab0e9691ce72");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b9e8dc46-3a0a-4b60-af83-fd398a7687ac");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ef77134b-a2e3-4aba-88db-b55e7329f834");
        }
    }
}
