using CampusPlacementManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusPlacementManagement.Data.Configurations;

public class ApplicationStatusHistoryConfiguration
    : IEntityTypeConfiguration<ApplicationStatusHistory>
{
    public void Configure(EntityTypeBuilder<ApplicationStatusHistory> builder)
    {
        builder.HasKey(h => h.HistoryId);

        builder.Property(h => h.PreviousStatus)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(h => h.NewStatus)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(h => h.UpdatedByUserId)
            .IsRequired();

        builder.Property(h => h.Remarks)
            .HasMaxLength(500);

        builder.Property(h => h.UpdatedAt)
            .IsRequired();

        // Job application relationship
        builder.HasOne(h => h.Application)
            .WithMany()
            .HasForeignKey(h => h.ApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        // User relationship
        builder.HasOne(h => h.UpdatedByUser)
            .WithMany()
            .HasForeignKey(h => h.UpdatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(h => h.ApplicationId);
        builder.HasIndex(h => h.UpdatedByUserId);
        builder.HasIndex(h => h.UpdatedAt);
    }
}
