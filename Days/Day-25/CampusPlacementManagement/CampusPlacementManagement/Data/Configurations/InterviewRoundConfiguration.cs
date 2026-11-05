using CampusPlacementManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusPlacementManagement.Data.Configurations;

public class InterviewRoundConfiguration
    : IEntityTypeConfiguration<InterviewRound>
{
    public void Configure(EntityTypeBuilder<InterviewRound> builder)
    {
        builder.HasKey(r => r.InterviewRoundId);

        builder.Property(r => r.RoundName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(r => r.Description);

        // Job relationship
        builder.HasOne(r => r.Job)
            .WithMany()
            .HasForeignKey(r => r.JobId)
            .OnDelete(DeleteBehavior.Restrict);

        // Prevent duplicate round numbers for the same job
        builder.HasIndex(r => new { r.JobId, r.RoundNumber })
            .IsUnique();

        builder.HasIndex(r => r.JobId);
    }
}
