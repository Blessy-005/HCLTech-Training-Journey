using CampusPlacementManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusPlacementManagement.Data.Configurations;

public class PlacementOfficerConfiguration
    : IEntityTypeConfiguration<PlacementOfficer>
{
    public void Configure(EntityTypeBuilder<PlacementOfficer> builder)
    {
        builder.HasKey(p => p.PlacementOfficerId);

        builder.Property(p => p.Designation)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        // One ApplicationUser can have one PlacementOfficer profile.
        builder.HasOne(p => p.User)
            .WithOne()
            .HasForeignKey<PlacementOfficer>(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // One College can have multiple PlacementOfficers.
        builder.HasOne(p => p.College)
            .WithMany()
            .HasForeignKey(p => p.CollegeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Optional relationship to the user who approved the officer.
        builder.HasOne(p => p.Approver)
            .WithMany()
            .HasForeignKey(p => p.ApprovedBy)
            .OnDelete(DeleteBehavior.Restrict);

        // UserId must be unique.
        builder.HasIndex(p => p.UserId)
            .IsUnique();
    }
}