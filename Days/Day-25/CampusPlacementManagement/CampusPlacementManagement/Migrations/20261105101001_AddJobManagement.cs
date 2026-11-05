using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusPlacementManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddJobManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobOpenings",
                columns: table => new
                {
                    JobId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedByHRId = table.Column<int>(type: "int", nullable: false),
                    JobTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    JobDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JobType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SalaryPackage = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    MinimumCGPA = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: true),
                    MaximumBacklogs = table.Column<int>(type: "int", nullable: true),
                    ApplicationDeadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsOpenToAllColleges = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobOpenings", x => x.JobId);
                    table.ForeignKey(
                        name: "FK_JobOpenings_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobOpenings_CompanyHRs_CreatedByHRId",
                        column: x => x.CreatedByHRId,
                        principalTable: "CompanyHRs",
                        principalColumn: "CompanyHRId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobTargetColleges",
                columns: table => new
                {
                    JobTargetCollegeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobId = table.Column<int>(type: "int", nullable: false),
                    CollegeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobTargetColleges", x => x.JobTargetCollegeId);
                    table.ForeignKey(
                        name: "FK_JobTargetColleges_Colleges_CollegeId",
                        column: x => x.CollegeId,
                        principalTable: "Colleges",
                        principalColumn: "CollegeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobTargetColleges_JobOpenings_JobId",
                        column: x => x.JobId,
                        principalTable: "JobOpenings",
                        principalColumn: "JobId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobOpenings_ApplicationDeadline",
                table: "JobOpenings",
                column: "ApplicationDeadline");

            migrationBuilder.CreateIndex(
                name: "IX_JobOpenings_CompanyId",
                table: "JobOpenings",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_JobOpenings_CreatedByHRId",
                table: "JobOpenings",
                column: "CreatedByHRId");

            migrationBuilder.CreateIndex(
                name: "IX_JobTargetColleges_CollegeId",
                table: "JobTargetColleges",
                column: "CollegeId");

            migrationBuilder.CreateIndex(
                name: "IX_JobTargetColleges_JobId_CollegeId",
                table: "JobTargetColleges",
                columns: new[] { "JobId", "CollegeId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobTargetColleges");

            migrationBuilder.DropTable(
                name: "JobOpenings");
        }
    }
}
