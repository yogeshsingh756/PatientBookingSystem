using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatientBookingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCollanglatAdd123 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserProfileImageUrl",
                table: "Users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "UserProfileImageUrl",
                table: "OtpVerifications",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserProfileImageUrl",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserProfileImageUrl",
                table: "OtpVerifications");
        }
    }
}
