namespace MovieTicketBooking.Models
{
    public class BookingSeat
    {
        public int Id { get; set; }

        public int BookingId { get; set; }

        public int ShowSeatId { get; set; }

        public decimal Price { get; set; }

        // Navigation properties
        public Booking? Booking { get; set; }

        public ShowSeat? ShowSeat { get; set; }
    }
}