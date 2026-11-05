
using CampusPlacementManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusPlacementManagement.Data.Configurations;

public class RecruitmentDriveConfiguration
    : IEntityTypeConfiguration<RecruitmentDrive>
{
    public void Configure(EntityTypeBuilder<RecruitmentDrive> builder)
    {
        builder.HasKey(d => d.DriveId);

        builder.Property(d => d.Venue)
            .HasMaxLength(255);

        builder.Property(d => d.Description);

        builder.Property(d => d.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        // Job relationship
        builder.HasOne(d => d.Job)
            .WithMany()
            .HasForeignKey(d => d.JobId)
            .OnDelete(DeleteBehavior.Restrict);

        // College relationship
        builder.HasOne(d => d.College)
            .WithMany()
            .HasForeignKey(d => d.CollegeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(d => d.JobId);
        builder.HasIndex(d => d.CollegeId);
        builder.HasIndex(d => d.DriveDate);
    }
}
