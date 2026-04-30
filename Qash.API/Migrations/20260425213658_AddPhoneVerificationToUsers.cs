using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Qash.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPhoneVerificationToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPhoneNumberVerified",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPhoneNumberVerified",
                table: "Users");
        }
    }
}
