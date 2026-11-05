using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusPlacementManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddRecruitmentDrives : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobApplication_JobOpenings_JobId",
                table: "JobApplication");

            migrationBuilder.DropForeignKey(
                name: "FK_JobApplication_Students_StudentId",
                table: "JobApplication");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobApplication",
                table: "JobApplication");

            migrationBuilder.RenameTable(
                name: "JobApplication",
                newName: "JobApplications");

            migrationBuilder.RenameIndex(
                name: "IX_JobApplication_StudentId_JobId",
                table: "JobApplications",
                newName: "IX_JobApplications_StudentId_JobId");

            migrationBuilder.RenameIndex(
                name: "IX_JobApplication_Status",
                table: "JobApplications",
                newName: "IX_JobApplications_Status");

            migrationBuilder.RenameIndex(
                name: "IX_JobApplication_JobId",
                table: "JobApplications",
                newName: "IX_JobApplications_JobId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobApplications",
                table: "JobApplications",
                column: "ApplicationId");

            migrationBuilder.CreateTable(
                name: "RecruitmentDrives",
                columns: table => new
                {
                    DriveId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobId = table.Column<int>(type: "int", nullable: false),
                    CollegeId = table.Column<int>(type: "int", nullable: false),
                    DriveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Venue = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecruitmentDrives", x => x.DriveId);
                    table.ForeignKey(
                        name: "FK_RecruitmentDrives_Colleges_CollegeId",
                        column: x => x.CollegeId,
                        principalTable: "Colleges",
                        principalColumn: "CollegeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecruitmentDrives_JobOpenings_JobId",
                        column: x => x.JobId,
                        principalTable: "JobOpenings",
                        principalColumn: "JobId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecruitmentDrives_CollegeId",
                table: "RecruitmentDrives",
                column: "CollegeId");

            migrationBuilder.CreateIndex(
                name: "IX_RecruitmentDrives_DriveDate",
                table: "RecruitmentDrives",
                column: "DriveDate");

            migrationBuilder.CreateIndex(
                name: "IX_RecruitmentDrives_JobId",
                table: "RecruitmentDrives",
                column: "JobId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplications_JobOpenings_JobId",
                table: "JobApplications",
                column: "JobId",
                principalTable: "JobOpenings",
                principalColumn: "JobId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplications_Students_StudentId",
                table: "JobApplications",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobApplications_JobOpenings_JobId",
                table: "JobApplications");

            migrationBuilder.DropForeignKey(
                name: "FK_JobApplications_Students_StudentId",
                table: "JobApplications");

            migrationBuilder.DropTable(
                name: "RecruitmentDrives");

            migrationBuilder.DropPrimaryKey(
                name: "PK_JobApplications",
                table: "JobApplications");

            migrationBuilder.RenameTable(
                name: "JobApplications",
                newName: "JobApplication");

            migrationBuilder.RenameIndex(
                name: "IX_JobApplications_StudentId_JobId",
                table: "JobApplication",
                newName: "IX_JobApplication_StudentId_JobId");

            migrationBuilder.RenameIndex(
                name: "IX_JobApplications_Status",
                table: "JobApplication",
                newName: "IX_JobApplication_Status");

            migrationBuilder.RenameIndex(
                name: "IX_JobApplications_JobId",
                table: "JobApplication",
                newName: "IX_JobApplication_JobId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_JobApplication",
                table: "JobApplication",
                column: "ApplicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplication_JobOpenings_JobId",
                table: "JobApplication",
                column: "JobId",
                principalTable: "JobOpenings",
                principalColumn: "JobId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplication_Students_StudentId",
                table: "JobApplication",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
