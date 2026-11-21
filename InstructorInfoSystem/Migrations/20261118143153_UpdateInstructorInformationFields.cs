
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstructorInfoSystem.Migrations
{
    public partial class UpdateInstructorInformationFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create FullName first so existing names can be preserved.
            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Instructors",
                type: "TEXT",
                nullable: true);

            // Combine the existing first and last names.
            migrationBuilder.Sql(
                @"UPDATE Instructors
                  SET FullName = trim(FirstName || ' ' || LastName);");

            // Make FullName required after copying the existing names.
            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Instructors",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            // Rename Major to Majorship.
            migrationBuilder.RenameColumn(
                name: "Major",
                table: "Instructors",
                newName: "Majorship");

            // Add new fields without reusing unrelated old columns.
            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "Instructors",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Instructors",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Designation",
                table: "Instructors",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BachelorDegree",
                table: "Instructors",
                type: "TEXT",
                maxLength: 150,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Instructors");

            migrationBuilder.DropColumn(
                name: "Age",
                table: "Instructors");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Instructors");

            migrationBuilder.DropColumn(
                name: "Designation",
                table: "Instructors");

            migrationBuilder.DropColumn(
                name: "BachelorDegree",
                table: "Instructors");

            migrationBuilder.RenameColumn(
                name: "Majorship",
                table: "Instructors",
                newName: "Major");
        }
    }
}
