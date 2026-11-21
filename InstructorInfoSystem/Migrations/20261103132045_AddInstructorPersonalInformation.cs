using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstructorInfoSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddInstructorPersonalInformation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Instructors",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactNumber",
                table: "Instructors",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Institute",
                table: "Instructors",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InstructorNumber",
                table: "Instructors",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Major",
                table: "Instructors",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Instructors");

            migrationBuilder.DropColumn(
                name: "ContactNumber",
                table: "Instructors");

            migrationBuilder.DropColumn(
                name: "Institute",
                table: "Instructors");

            migrationBuilder.DropColumn(
                name: "InstructorNumber",
                table: "Instructors");

            migrationBuilder.DropColumn(
                name: "Major",
                table: "Instructors");
        }
    }
}
