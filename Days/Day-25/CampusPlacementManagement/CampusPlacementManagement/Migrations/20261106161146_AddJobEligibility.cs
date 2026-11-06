using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusPlacementManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddJobEligibility : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobEligibleDepartments",
                columns: table => new
                {
                    JobEligibleDepartmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobId = table.Column<int>(type: "int", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobEligibleDepartments", x => x.JobEligibleDepartmentId);
                    table.ForeignKey(
                        name: "FK_JobEligibleDepartments_JobOpenings_JobId",
                        column: x => x.JobId,
                        principalTable: "JobOpenings",
                        principalColumn: "JobId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobEligibleGraduationYears",
                columns: table => new
                {
                    JobEligibleGraduationYearId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobId = table.Column<int>(type: "int", nullable: false),
                    GraduationYear = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobEligibleGraduationYears", x => x.JobEligibleGraduationYearId);
                    table.ForeignKey(
                        name: "FK_JobEligibleGraduationYears_JobOpenings_JobId",
                        column: x => x.JobId,
                        principalTable: "JobOpenings",
                        principalColumn: "JobId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobEligibleDepartments_JobId_Department",
                table: "JobEligibleDepartments",
                columns: new[] { "JobId", "Department" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobEligibleGraduationYears_JobId_GraduationYear",
                table: "JobEligibleGraduationYears",
                columns: new[] { "JobId", "GraduationYear" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobEligibleDepartments");

            migrationBuilder.DropTable(
                name: "JobEligibleGraduationYears");
        }
    }
}
