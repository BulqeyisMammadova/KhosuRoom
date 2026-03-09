using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KhosuRoom.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddMeetLinkAndRefreshTokenToGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GoogleRefreshToken",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoogleRefreshToken",
                table: "Groups");
        }
    }
}
