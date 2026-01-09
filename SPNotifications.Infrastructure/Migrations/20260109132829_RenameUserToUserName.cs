using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SPNotifications.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameUserToUserName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Username",
                table: "Notifications");
        }
    }
}
