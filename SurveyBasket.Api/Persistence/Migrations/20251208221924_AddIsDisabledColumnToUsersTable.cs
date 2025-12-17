using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyBasket.Api.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDisabledColumnToUsersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDisabled",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9E0533B3-89D8-4E14-9392-336C84BBC37C",
                columns: new[] { "IsDisabled", "PasswordHash" },
                values: new object[] { false, "AQAAAAIAAYagAAAAEDOj8gBcHmE3aNCfjPae2EMNndxH29g47QkKgqrrnZJVZRrvbxB3K4DdXHLbZNHHTQ==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDisabled",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9E0533B3-89D8-4E14-9392-336C84BBC37C",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEBPj1X43plpRvWVDilXQRj4KGiYYKb1ZnFwjAIawB6sYnII8pDNGYEYvl1Gn77Z1aA==");
        }
    }
}
