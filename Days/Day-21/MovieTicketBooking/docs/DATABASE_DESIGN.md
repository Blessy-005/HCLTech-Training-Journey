# Database Design - Movie Ticket Booking System

## 1. Database Information

- **Database:** MovieTicketBookingDb
- **Database Server:** SQL Server Express (.\SQLEXPRESS)
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
- Each BookingSeat references one ShowSeat.
- A ShowSeat can be included in a BookingSeat only once.

## 4. Important Constraints and Indexes

The following constraints are configured through Entity Framework Core:

| Table | Constraint |
|---|---|
| Seats | Unique combination of ScreenId and SeatNumber |
| ShowSeats | Unique combination of ShowId and SeatId |
| BookingSeats | Unique combination of BookingId and ShowSeatId |

Additional database rules:

- Required fields and maximum column lengths are configured.
- Ticket prices and booking amounts use decimal precision (10,2).
- Foreign keys maintain relationships between entities.
- Restrict delete behavior protects related records from accidental deletion.

## 5. Seat Availability Design

The Seats table stores physical seats belonging to a screen.

The ShowSeats table stores the availability of each physical seat for a particular show.

Example:

| Show | Seat | Status |
|---|---|---|
| Morning Show | A1 | Available |
| Afternoon Show | A1 | Booked |
| Evening Show | A1 | Available |

Each show receives its own ShowSeat records when the show is created.

Possible seat statuses include Available and Booked.

## 6. Booking Structure

The Bookings table stores:

- Customer name
- Customer email
- Booking date
- Total amount
- Booking status

The BookingSeats table stores the individual ShowSeat records included in a booking, along with the price charged for each seat.

The application calculates the total booking amount using the show ticket price. The client cannot determine the final amount by submitting a price.

## 7. Booking Transactions

Booking creation uses a database transaction to:

1. Validate the selected show seats.
2. Verify that all selected seats are available and belong to the same show.
3. Calculate the total amount on the server.
4. Create the booking and its BookingSeat records.
5. Mark the selected ShowSeats as Booked.

Cancellation also uses a database transaction to:

1. Verify that the booking exists and is confirmed.
2. Mark its ShowSeats as Available.
3. Change the booking status to Cancelled.
4. Preserve the booking and booking-seat history.

## 8. Entity Framework Core

ApplicationDbContext manages all eight entities through DbSet properties.

Entity relationships, indexes, column lengths, decimal precision, and delete behavior are configured in OnModelCreating.

EF Core migrations are used to create and update the SQL Server database schema.
