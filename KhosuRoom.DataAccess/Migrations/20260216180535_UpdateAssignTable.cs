using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KhosuRoom.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAssignTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_AspNetUsers_AppUserId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Groups_GroupId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Groups_GroupId1",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_AspNetUsers_AppUserId",
                table: "Submissions");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_AppUserId",
                table: "Assignments");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_GroupId_DueDate",
                table: "Assignments");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_GroupId1",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "SubmittedDate",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "Text",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "GroupId1",
                table: "Assignments");

            migrationBuilder.RenameColumn(
                name: "LastUpdatedAtUtc",
                table: "Submissions",
                newName: "SubmittedAt");

            migrationBuilder.RenameColumn(
                name: "AppUserId",
                table: "Submissions",
                newName: "GradedByTeacherId");

            migrationBuilder.RenameIndex(
                name: "IX_Submissions_AppUserId",
                table: "Submissions",
                newName: "IX_Submissions_GradedByTeacherId");

            migrationBuilder.AlterColumn<decimal>(
                name: "Grade",
                table: "Submissions",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Feedback",
                table: "Submissions",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Submissions",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GradedAt",
                table: "Submissions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Submissions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "FileUrl",
                table: "SubmissionAttachments",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "FileUrl",
                table: "AssignmentAttachments",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "AssignmentAttachments",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_GroupId",
                table: "Assignments",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Groups_GroupId",
                table: "Assignments",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_AspNetUsers_GradedByTeacherId",
                table: "Submissions",
                column: "GradedByTeacherId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Groups_GroupId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_AspNetUsers_GradedByTeacherId",
                table: "Submissions");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_GroupId",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "GradedAt",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Submissions");

            migrationBuilder.RenameColumn(
                name: "SubmittedAt",
                table: "Submissions",
                newName: "LastUpdatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "GradedByTeacherId",
                table: "Submissions",
                newName: "AppUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Submissions_GradedByTeacherId",
                table: "Submissions",
                newName: "IX_Submissions_AppUserId");

            migrationBuilder.AlterColumn<int>(
                name: "Grade",
                table: "Submissions",
                type: "int",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Feedback",
                table: "Submissions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedDate",
                table: "Submissions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "Submissions",
                type: "nvarchar(max)",
                maxLength: 8000,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileUrl",
                table: "SubmissionAttachments",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AddColumn<Guid>(
                name: "AppUserId",
                table: "Assignments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "GroupId1",
                table: "Assignments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileUrl",
                table: "AssignmentAttachments",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "AssignmentAttachments",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_AppUserId",
                table: "Assignments",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_GroupId_DueDate",
                table: "Assignments",
                columns: new[] { "GroupId", "DueDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_GroupId1",
                table: "Assignments",
                column: "GroupId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_AspNetUsers_AppUserId",
                table: "Assignments",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Groups_GroupId",
                table: "Assignments",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Groups_GroupId1",
                table: "Assignments",
                column: "GroupId1",
                principalTable: "Groups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_AspNetUsers_AppUserId",
                table: "Submissions",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
