using CampusPlacementManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CampusPlacementManagement.Data.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasKey(s => s.StudentId);

        builder.Property(s => s.RegisterNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.Department)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Degree)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.CGPA)
            .HasPrecision(4, 2);

        builder.Property(s => s.ResumeUrl)
            .HasMaxLength(500);

        builder.Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        // Register number must be unique within a college.
        builder.HasIndex(s => new { s.CollegeId, s.RegisterNumber })
            .IsUnique();

        // One ApplicationUser has one Student profile.
        builder.HasOne(s => s.User)
            .WithOne()
            .HasForeignKey<Student>(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => s.UserId)
            .IsUnique();

        // One College can have multiple Students.
        builder.HasOne(s => s.College)
            .WithMany()
            .HasForeignKey(s => s.CollegeId)
            .OnDelete(DeleteBehavior.Restrict);

        // One PlacementOfficer can manage multiple Students.
        builder.HasOne(s => s.PlacementOfficer)
            .WithMany()
            .HasForeignKey(s => s.PlacementOfficerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}