using CampusPlacementManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusPlacementManagement.Data.Configurations;

public class JobEligibleDepartmentConfiguration
    : IEntityTypeConfiguration<JobEligibleDepartment>
{
    public void Configure(EntityTypeBuilder<JobEligibleDepartment> builder)
    {
        builder.HasKey(d => d.JobEligibleDepartmentId);

        builder.Property(d => d.Department)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(d => d.Job)
            .WithMany(j => j.EligibleDepartments)
            .HasForeignKey(d => d.JobId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(d => new { d.JobId, d.Department })
            .IsUnique();
    }
}
