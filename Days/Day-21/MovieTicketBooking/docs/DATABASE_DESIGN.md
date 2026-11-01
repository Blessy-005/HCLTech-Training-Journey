# Database Design – Movie Ticket Booking System

## 1. Database Information

- **Database:** MovieTicketBookingDb
- **Database Server:** SQL Server Express
- **ORM:** Entity Framework Core 10
- **DbContext:** ApplicationDbContext

## 2. Database Tables

The application contains eight main tables.

| Table | Purpose |
|---|---|
| Movies | Stores movie information |
| Theatres | Stores theatre information |
| Screens | Stores screens belonging to theatres |
| Seats | Stores physical seats belonging to screens |
| Shows | Stores movie show schedules |
| ShowSeats | Tracks seat availability for each show |
| Bookings | Stores customer booking details |
| BookingSeats | Stores the seats included in each booking |

## 3. Table Relationships

- One Theatre can have multiple Screens.
- One Screen can have multiple Seats.
- One Movie can have multiple Shows.
- One Screen can host multiple Shows at different times.
- One Show has multiple ShowSeats.
- One physical Seat can appear in multiple Shows through ShowSeats.
- One Booking can contain multiple BookingSeats.
- Each BookingSeat refers to one ShowSeat.

## 4. Important Constraints

- Seat numbers must be unique within a screen.
- A physical seat can appear only once for a particular show.
- A ShowSeat can appear only once within a booking.
- Prices use decimal precision of 10,2.
- Foreign keys protect relationships between tables.
- Restrict delete behavior protects related records from accidental deletion.

## 5. Seat Availability

The Seats table stores physical seats.

The ShowSeats table stores the availability of those seats for individual shows.

Example:

| Show | Seat | Status |
|---|---|---|
| Morning Show | A1 | Available |
| Afternoon Show | A1 | Booked |
| Evening Show | A1 | Available |

## 6. Booking Structure

The Bookings table stores customer and booking information.

The BookingSeats table stores the individual show seats included in a booking.

The total booking amount is calculated by the application and stored in the Bookings table.

## 7. Entity Framework Core

ApplicationDbContext manages all eight entities through DbSet properties.

Entity relationships, indexes, column lengths, decimal precision, and delete behavior are configured in OnModelCreating.

EF Core migrations will be used to create and update the SQL Server database schema.