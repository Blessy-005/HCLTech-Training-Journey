using CampusPlacementManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusPlacementManagement.Data.Configurations;

public class CompanyHRConfiguration : IEntityTypeConfiguration<CompanyHR>
{
    public void Configure(EntityTypeBuilder<CompanyHR> builder)
    {
        builder.HasKey(h => h.CompanyHRId);

        builder.Property(h => h.Designation)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(h => h.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        // One ApplicationUser has one CompanyHR profile.
        builder.HasOne(h => h.User)
            .WithOne()
            .HasForeignKey<CompanyHR>(h => h.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // One Company can have multiple HR users.
        builder.HasOne(h => h.Company)
            .WithMany()
            .HasForeignKey(h => h.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Optional relationship to the user who approved the HR.
        builder.HasOne(h => h.Approver)
            .WithMany()
            .HasForeignKey(h => h.ApprovedBy)
            .OnDelete(DeleteBehavior.Restrict);

        // A user can have only one CompanyHR profile.
        builder.HasIndex(h => h.UserId)
            .IsUnique();
    }
}