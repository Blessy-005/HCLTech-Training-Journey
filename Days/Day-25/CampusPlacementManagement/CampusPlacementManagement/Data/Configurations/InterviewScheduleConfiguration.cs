using CampusPlacementManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusPlacementManagement.Data.Configurations;

public class InterviewScheduleConfiguration
    : IEntityTypeConfiguration<InterviewSchedule>
{
    public void Configure(EntityTypeBuilder<InterviewSchedule> builder)
    {
        builder.HasKey(s => s.InterviewScheduleId);

        builder.Property(s => s.MeetingLink)
            .HasMaxLength(500);

        builder.Property(s => s.Venue)
            .HasMaxLength(255);

        builder.Property(s => s.Feedback);

        builder.Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        // Job application relationship
        builder.HasOne(s => s.Application)
            .WithMany()
            .HasForeignKey(s => s.ApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Interview round relationship
        builder.HasOne(s => s.InterviewRound)
            .WithMany()
            .HasForeignKey(s => s.InterviewRoundId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(s => s.ApplicationId);
        builder.HasIndex(s => s.InterviewRoundId);
        builder.HasIndex(s => s.ScheduledAt);
    }
}
