using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolMedicalSystem.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddBmiToHealthRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Bmi",
                table: "HealthRecords",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "BmiStatus",
                table: "HealthRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bmi",
                table: "HealthRecords");

            migrationBuilder.DropColumn(
                name: "BmiStatus",
                table: "HealthRecords");
        }
    }
}
