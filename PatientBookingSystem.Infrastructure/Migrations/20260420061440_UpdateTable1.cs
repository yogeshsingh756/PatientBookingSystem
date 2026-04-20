using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatientBookingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTable1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PatientAppointmentId",
                table: "PatientAppointmentStatusHistories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientAppointmentStatusHistories_PatientAppointmentId",
                table: "PatientAppointmentStatusHistories",
                column: "PatientAppointmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientAppointmentStatusHistories_PatientAppointments_Patien~",
                table: "PatientAppointmentStatusHistories",
                column: "PatientAppointmentId",
                principalTable: "PatientAppointments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientAppointmentStatusHistories_PatientAppointments_Patien~",
                table: "PatientAppointmentStatusHistories");

            migrationBuilder.DropIndex(
                name: "IX_PatientAppointmentStatusHistories_PatientAppointmentId",
                table: "PatientAppointmentStatusHistories");

            migrationBuilder.DropColumn(
                name: "PatientAppointmentId",
                table: "PatientAppointmentStatusHistories");
        }
    }
}
