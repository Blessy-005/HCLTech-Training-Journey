namespace MovieTicketBooking.DTOs.Bookings
{
    public class BookingResponseDto
    {
        public int Id { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string CustomerEmail { get; set; } = string.Empty;

        public DateTime BookingDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = string.Empty;

        public List<BookingSeatResponseDto> Seats { get; set; } = new();
    }

    public class BookingSeatResponseDto
    {
        public int Id { get; set; }

        public int ShowSeatId { get; set; }

        public string SeatNumber { get; set; } = string.Empty;

        public decimal Price { get; set; }
    }
}