# Movie Ticket Booking System - Validation Rules

## 1. General API Validation

- Required fields must be provided.
- String fields must respect their configured maximum lengths.
- Numeric fields must satisfy their configured ranges.
- Invalid DTO requests are rejected by ASP.NET Core model validation.
- Business rules are enforced by the service layer.

## 2. Movies

- Title is required and limited to 150 characters.
- Language and Genre are limited to 50 characters.
- DurationMinutes must be between 1 and 500.
- PosterUrl is limited to 500 characters.
- Only active movies are returned by the normal movie listing.

## 3. Theatres

- Name is required and limited to 150 characters.
- Address is required and limited to 300 characters.
- City is required and limited to 100 characters.
- Only active theatres are returned by the normal theatre listing.

## 4. Screens

- Name is required and limited to 100 characters.
- TheatreId must be a positive integer.
- TotalSeats must be between 1 and 1000.
- The referenced theatre must exist and be active.

## 5. Seats

- SeatNumber is required and limited to 10 characters.
- SeatType is required and limited to 30 characters.
- ScreenId must be a positive integer.
- The referenced screen must exist and be active.
- Seat numbers are trimmed and converted to uppercase.
- Seat numbers must be unique within the same screen.
- Bulk creation rejects an empty collection and duplicate seat numbers.
- A seat assigned to a show cannot be modified or deactivated.

## 6. Shows

- MovieId and ScreenId must be positive integers.
- StartTime and EndTime are required.
- EndTime must be later than StartTime.
- TicketPrice must be between 0.01 and 100000.
- The referenced movie and screen must exist and be active.
- Show duration must match the movie duration.
- Active shows on the same screen must not overlap.
- A show with booking history cannot have its movie, screen, or schedule changed.
- A show with booking history cannot be deactivated.

## 7. Show Seats

- ShowId must be a positive integer.
- ShowSeat records are generated when a show is created.
- A physical seat can have only one ShowSeat record per show.
- A ShowSeat must be Available before it can be booked.
- Seat status is maintained separately for each show.

## 8. Bookings

- CustomerName is required and limited to 150 characters.
- CustomerEmail is required, must have a valid email format, and is limited to 150 characters.
- At least one ShowSeat ID must be submitted.
- Selected ShowSeat IDs must be positive and unique.
- Every selected ShowSeat must exist.
- All selected seats must belong to the same show.
- The show must be active and in the future.
- All selected ShowSeats must have status Available.
- The booking amount is calculated by the server using the show ticket price.
- Booking creation is performed within a database transaction.
- Only Confirmed bookings can be cancelled.
- Cancellation releases the associated ShowSeats and preserves booking history.

## 9. Database Constraints

- Seats: unique index on (ScreenId, SeatNumber).
- ShowSeats: unique index on (ShowId, SeatId).
- BookingSeats: unique index on (BookingId, ShowSeatId).
- Foreign keys enforce entity relationships.
- Restrict delete behavior protects related records.

## 10. Error Handling

- Invalid model input is handled by ASP.NET Core API model validation.
- ArgumentException is converted to HTTP 400 by centralized exception middleware.
- Unexpected exceptions return HTTP 500 with a generic error message.
- Internal exception details are not exposed in the HTTP response.
