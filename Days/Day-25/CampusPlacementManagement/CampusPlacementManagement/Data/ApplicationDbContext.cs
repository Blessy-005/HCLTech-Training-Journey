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
    public DbSet<JobOpening> JobOpenings { get; set; }

    public DbSet<JobTargetCollege> JobTargetColleges { get; set; }
    public DbSet<JobApplication> JobApplications { get; set; }
    // Recruitment drive management
    public DbSet<RecruitmentDrive> RecruitmentDrives { get; set; }
    // Load Fluent API configurations
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);
    }
}