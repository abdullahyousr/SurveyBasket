using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SurveyBasket.Api.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class seedIdentityTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsDefault", "IsDeleted", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "DFC22A93-7F04-4F75-AF14-1C8CA3F6AF21", "7A709DFB-1294-4261-9A45-1D62441F9547", true, false, "Member", "MEMBER" },
                    { "E6C0EF61-4A63-46CF-A7BA-8A9F3BDE5C37", "79A95E4C-425E-41ED-9C5F-9255CCE70200", false, false, "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "9E0533B3-89D8-4E14-9392-336C84BBC37C", 0, "6D17FE54-F5E1-43B6-B3E0-CFF6277AF8F5", "admin@survey-basket.com", true, "Survey Basket", "Admin", false, null, "ADMIN@SURVEY-BASKET.COM", "ADMIN@SURVEY-BASKET.COM", "AQAAAAIAAYagAAAAECyG0Ns8OyTPSKOBpFObMCRYnZmGKQt5A9FeVXewoVs5ygX/VoabNyFg3KyybcK6Vg==", null, false, "D630DC5170A34F9BBDA044C98845F7F9", false, "admin@survey-basket.com" });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 1, "Permissions", "polls.read", "E6C0EF61-4A63-46CF-A7BA-8A9F3BDE5C37" },
                    { 2, "Permissions", "polls.add", "E6C0EF61-4A63-46CF-A7BA-8A9F3BDE5C37" },
                    { 3, "Permissions", "polls.update", "E6C0EF61-4A63-46CF-A7BA-8A9F3BDE5C37" },
                    { 4, "Permissions", "polls.delete", "E6C0EF61-4A63-46CF-A7BA-8A9F3BDE5C37" },
                    { 5, "Permissions", "questions.read", "E6C0EF61-4A63-46CF-A7BA-8A9F3BDE5C37" },
                    { 6, "Permissions", "questions.add", "E6C0EF61-4A63-46CF-A7BA-8A9F3BDE5C37" },
                    { 7, "Permissions", "questions.update", "E6C0EF61-4A63-46CF-A7BA-8A9F3BDE5C37" },
                    { 8, "Permissions", "users.read", "E6C0EF61-4A63-46CF-A7BA-8A9F3BDE5C37" },
                    { 9, "Permissions", "users.add", "E6C0EF61-4A63-46CF-A7BA-8A9F3BDE5C37" },
                    { 10, "Permissions", "users.update", "E6C0EF61-4A63-46CF-A7BA-8A9F3BDE5C37" },
                    { 11, "Permissions", "roles.read", "E6C0EF61-4A63-46CF-A7BA-8A9F3BDE5C37" },
                    { 12, "Permissions", "roles.add", "E6C0EF61-4A63-46CF-A7BA-8A9F3BDE5C37" },
                    { 13, "Permissions", "roles.update", "E6C0EF61-4A63-46CF-A7BA-8A9F3BDE5C37" },
                    { 14, "Permissions", "results.read", "E6C0EF61-4A63-46CF-A7BA-8A9F3BDE5C37" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "E6C0EF61-4A63-46CF-A7BA-8A9F3BDE5C37", "9E0533B3-89D8-4E14-9392-336C84BBC37C" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "DFC22A93-7F04-4F75-AF14-1C8CA3F6AF21");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "E6C0EF61-4A63-46CF-A7BA-8A9F3BDE5C37", "9E0533B3-89D8-4E14-9392-336C84BBC37C" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "E6C0EF61-4A63-46CF-A7BA-8A9F3BDE5C37");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9E0533B3-89D8-4E14-9392-336C84BBC37C");
        }
    }
}
