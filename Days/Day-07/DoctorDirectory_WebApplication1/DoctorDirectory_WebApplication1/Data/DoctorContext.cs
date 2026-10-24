
using DoctorDirectory.Models;
using Microsoft.EntityFrameworkCore;

namespace DoctorDirectory.Data
{
    public class DoctorContext : DbContext
    {
        public DoctorContext(
            DbContextOptions<DoctorContext> options)
            : base(options)
        {
        }

        public DbSet<Doctor> Doctors { get; set; }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Doctor>()
                .HasIndex(d => d.Email)
                .IsUnique();

            modelBuilder.Entity<Doctor>()
                .Property(d => d.ConsultationFee)
                .HasPrecision(10, 2);
        }
    }
}
