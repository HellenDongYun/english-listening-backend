using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Listening.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSubtitleSegments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubtitleSegment_T_Exercises_ExerciseId",
                table: "SubtitleSegment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubtitleSegment",
                table: "SubtitleSegment");

            migrationBuilder.RenameTable(
                name: "SubtitleSegment",
                newName: "SubtitleSegments");

            migrationBuilder.RenameIndex(
                name: "IX_SubtitleSegment_ExerciseId",
                table: "SubtitleSegments",
                newName: "IX_SubtitleSegments_ExerciseId");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EndTime",
                table: "SubtitleSegments",
                type: "time(6)",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartTime",
                table: "SubtitleSegments",
                type: "time(6)",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "SubtitleSegments",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubtitleSegments",
                table: "SubtitleSegments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubtitleSegments_T_Exercises_ExerciseId",
                table: "SubtitleSegments",
                column: "ExerciseId",
                principalTable: "T_Exercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubtitleSegments_T_Exercises_ExerciseId",
                table: "SubtitleSegments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubtitleSegments",
                table: "SubtitleSegments");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "SubtitleSegments");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "SubtitleSegments");

            migrationBuilder.DropColumn(
                name: "Text",
                table: "SubtitleSegments");

            migrationBuilder.RenameTable(
                name: "SubtitleSegments",
                newName: "SubtitleSegment");

            migrationBuilder.RenameIndex(
                name: "IX_SubtitleSegments_ExerciseId",
                table: "SubtitleSegment",
                newName: "IX_SubtitleSegment_ExerciseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubtitleSegment",
                table: "SubtitleSegment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubtitleSegment_T_Exercises_ExerciseId",
                table: "SubtitleSegment",
                column: "ExerciseId",
                principalTable: "T_Exercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
