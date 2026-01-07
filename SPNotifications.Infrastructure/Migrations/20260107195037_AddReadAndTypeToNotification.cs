using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SPNotifications.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReadAndTypeToNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "User",
                table: "Notifications",
                newName: "Type");

            migrationBuilder.AddColumn<bool>(
                name: "Read",
                table: "Notifications",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Read",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Notifications",
                newName: "User");
        }
    }
}
