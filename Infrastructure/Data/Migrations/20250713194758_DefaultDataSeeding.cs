using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class DefaultDataSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "21a8f50b-cc48-4cc6-8c8c-9a96bf9c5ef8", null, "User", "USER" },
                    { "bedae896-605b-47f9-837f-a042993712d1", null, "Admin", "ADMIN" },
                    { "ed43d024-e9d5-4b71-84ff-06ef1104e8ef", null, "Moderator", "MODERATOR" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Biography", "ConcurrencyStamp", "Email", "EmailConfirmed", "LastLogin", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "ProfilePicture", "SecurityStamp", "TierId", "TwoFactorEnabled", "UserName" },
                values: new object[] { "eebefb34-6e4d-4bda-86bc-9eb148adb8bc", 0, null, "f2027726-9e34-4db5-a69a-b02dc12682db", "admin@example.com", true, new DateTimeOffset(new DateTime(2025, 7, 13, 19, 47, 58, 392, DateTimeKind.Unspecified).AddTicks(9256), new TimeSpan(0, 0, 0, 0, 0)), false, null, "ADMIN@EXAMPLE.COM", "ADMIN", "AQAAAAIAAYagAAAAEMU/0dT28eeZPy6Lm9PK3iGyx511fzz7aERKIhVYKGVUiIWoJZ5hhrYDc1HumLUlNQ==", null, false, null, "ef38f54b-3233-4f82-9db8-0c2b21a0dcd2", null, false, "Admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "bedae896-605b-47f9-837f-a042993712d1", "eebefb34-6e4d-4bda-86bc-9eb148adb8bc" });

            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[] { "Id", "Created", "CreatorId", "Description", "Name", "Picture" },
                values: new object[] { "0402c2f0-152f-4db8-b8cb-bbdc7b9afb2f", new DateTimeOffset(new DateTime(2025, 7, 13, 19, 47, 58, 392, DateTimeKind.Unspecified).AddTicks(9560), new TimeSpan(0, 0, 0, 0, 0)), "eebefb34-6e4d-4bda-86bc-9eb148adb8bc", "This is a test group", "Test group", null });

            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "Id", "Content", "Created", "GroupId", "Modified", "UserId" },
                values: new object[,]
                {
                    { "d9b1661a-37ca-4f9c-87df-c99ace2e6058", "Welcome to my app", new DateTimeOffset(new DateTime(2025, 7, 13, 19, 47, 58, 392, DateTimeKind.Unspecified).AddTicks(9590), new TimeSpan(0, 0, 0, 0, 0)), null, null, "eebefb34-6e4d-4bda-86bc-9eb148adb8bc" },
                    { "7daec7da-b161-4a8b-842e-9f2e0f0a79a4", "Welcome to my group", new DateTimeOffset(new DateTime(2025, 7, 13, 19, 47, 58, 392, DateTimeKind.Unspecified).AddTicks(9597), new TimeSpan(0, 0, 0, 0, 0)), "0402c2f0-152f-4db8-b8cb-bbdc7b9afb2f", null, "eebefb34-6e4d-4bda-86bc-9eb148adb8bc" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "21a8f50b-cc48-4cc6-8c8c-9a96bf9c5ef8");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ed43d024-e9d5-4b71-84ff-06ef1104e8ef");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "bedae896-605b-47f9-837f-a042993712d1", "eebefb34-6e4d-4bda-86bc-9eb148adb8bc" });

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: "7daec7da-b161-4a8b-842e-9f2e0f0a79a4");

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: "d9b1661a-37ca-4f9c-87df-c99ace2e6058");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bedae896-605b-47f9-837f-a042993712d1");

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: "0402c2f0-152f-4db8-b8cb-bbdc7b9afb2f");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "eebefb34-6e4d-4bda-86bc-9eb148adb8bc");
        }
    }
}
