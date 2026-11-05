using CampusPlacementManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusPlacementManagement.Data.Configurations;

public class JobTargetCollegeConfiguration
    : IEntityTypeConfiguration<JobTargetCollege>
{
    public void Configure(
        EntityTypeBuilder<JobTargetCollege> builder)
    {
        builder.HasKey(jtc => jtc.JobTargetCollegeId);

        // Job relationship
        builder.HasOne(jtc => jtc.Job)
            .WithMany()
            .HasForeignKey(jtc => jtc.JobId)
            .OnDelete(DeleteBehavior.Restrict);

        // College relationship
        builder.HasOne(jtc => jtc.College)
            .WithMany()
            .HasForeignKey(jtc => jtc.CollegeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Prevent duplicate college entries for the same job
        builder.HasIndex(jtc => new
        {
            jtc.JobId,
            jtc.CollegeId
        }).IsUnique();
    }
}