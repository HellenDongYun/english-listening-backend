using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Listening.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExerciseDuration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Audio_Id",
                table: "T_Exercises",
                newName: "AudioId");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Duration",
                table: "T_Exercises",
                type: "time(6)",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "T_Exercises");

            migrationBuilder.RenameColumn(
                name: "AudioId",
                table: "T_Exercises",
                newName: "Audio_Id");
        }
    }
}
