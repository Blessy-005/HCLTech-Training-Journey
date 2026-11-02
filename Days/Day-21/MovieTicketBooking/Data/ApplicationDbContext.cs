
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Database tables
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Theatre> Theatres { get; set; }
        public DbSet<Screen> Screens { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Show> Shows { get; set; }
        public DbSet<ShowSeat> ShowSeats { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingSeat> BookingSeats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Movie configuration
            modelBuilder.Entity<Movie>(entity =>
            {
                entity.Property(m => m.Title)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(m => m.Language)
                    .HasMaxLength(50);

                entity.Property(m => m.Genre)
                    .HasMaxLength(50);

                entity.Property(m => m.PosterUrl)
                    .HasMaxLength(500);
            });

            // Theatre configuration
            modelBuilder.Entity<Theatre>(entity =>
            {
                entity.Property(t => t.Name)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(t => t.Address)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(t => t.City)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            // Screen configuration
            modelBuilder.Entity<Screen>(entity =>
            {
                entity.Property(s => s.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(s => s.Theatre)
                    .WithMany()
                    .HasForeignKey(s => s.TheatreId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Seat configuration
            modelBuilder.Entity<Seat>(entity =>
            {
                entity.Property(s => s.SeatNumber)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(s => s.SeatType)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.HasIndex(s => new { s.ScreenId, s.SeatNumber })
                    .IsUnique();

                entity.HasOne(s => s.Screen)
                    .WithMany()
                    .HasForeignKey(s => s.ScreenId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Show configuration
            modelBuilder.Entity<Show>(entity =>
            {
                entity.Property(s => s.TicketPrice)
                    .HasPrecision(10, 2);

                entity.HasOne(s => s.Movie)
                    .WithMany()
                    .HasForeignKey(s => s.MovieId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.Screen)
                    .WithMany()
                    .HasForeignKey(s => s.ScreenId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ShowSeat configuration
            modelBuilder.Entity<ShowSeat>(entity =>
            {
                entity.Property(s => s.Status)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasIndex(s => new { s.ShowId, s.SeatId })
                    .IsUnique();

                entity.HasOne(s => s.Show)
                    .WithMany()
                    .HasForeignKey(s => s.ShowId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(s => s.Seat)
                    .WithMany()
                    .HasForeignKey(s => s.SeatId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Booking configuration
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.Property(b => b.CustomerName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(b => b.CustomerEmail)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(b => b.TotalAmount)
                    .HasPrecision(10, 2);

                entity.Property(b => b.Status)
                    .IsRequired()
                    .HasMaxLength(20);

                // Link booking to Identity user
                entity.HasOne(b => b.User)
                    .WithMany()
                    .HasForeignKey(b => b.UserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // BookingSeat configuration
            modelBuilder.Entity<BookingSeat>(entity =>
            {
                entity.Property(bs => bs.Price)
                    .HasPrecision(10, 2);

                entity.HasIndex(bs => new { bs.BookingId, bs.ShowSeatId })
                    .IsUnique();

                entity.HasOne(bs => bs.Booking)
                    .WithMany(b => b.BookingSeats)
                    .HasForeignKey(bs => bs.BookingId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(bs => bs.ShowSeat)
                    .WithMany()
                    .HasForeignKey(bs => bs.ShowSeatId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
