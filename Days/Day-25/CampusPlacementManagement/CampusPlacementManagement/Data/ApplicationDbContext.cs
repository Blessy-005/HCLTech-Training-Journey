using CampusPlacementManagement.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Core entity tables
    public DbSet<College> Colleges { get; set; }
    public DbSet<PlacementOfficer> PlacementOfficers { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<CompanyHR> CompanyHRs { get; set; }
    public DbSet<Student> Students { get; set; }

    // Job management
    public DbSet<JobOpening> JobOpenings { get; set; }
    public DbSet<JobTargetCollege> JobTargetColleges { get; set; }

    // Job application management
    public DbSet<JobApplication> JobApplications { get; set; }
    public DbSet<ApplicationStatusHistory> ApplicationStatusHistories { get; set; }

    // Recruitment drive management
    public DbSet<RecruitmentDrive> RecruitmentDrives { get; set; }

    // Interview management
    public DbSet<InterviewRound> InterviewRounds { get; set; }
    public DbSet<InterviewSchedule> InterviewSchedules { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);
    }
}

