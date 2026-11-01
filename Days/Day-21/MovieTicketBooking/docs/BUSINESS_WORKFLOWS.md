# Movie Ticket Booking System - Business Workflows

## 1. Movie Management

1. An administrator submits movie details.
2. The API validates the required fields.
3. The MovieService processes the request.
4. The MovieRepository saves the movie.
5. The API returns the created movie.

Movies are deactivated instead of physically deleted.

## 2. Theatre Management

1. Theatre details are submitted.
2. The API validates the theatre information.
3. The theatre is saved in the database.
4. The theatre can be retrieved, updated, or deactivated.

## 3. Screen Management

1. A screen is created with a name, theatre, and total seat count.
2. The service verifies that the selected theatre exists and is active.
3. The screen is saved with its theatre relationship.
4. Screens can be retrieved, updated, or deactivated.

## 4. Seat Management

1. A physical seat is created for a screen.
2. The service verifies that the screen exists and is active.
3. The seat number is normalized by trimming whitespace and converting it to uppercase.
4. Duplicate seat numbers within the same screen are rejected.
5. Multiple seats can also be created through the bulk creation endpoint.

Seats already assigned to shows cannot be modified or deactivated, preserving show and booking history.

## 5. Show Scheduling

1. A show is submitted with a movie, screen, start time, end time, and ticket price.
2. The service verifies that the movie and screen exist and are active.
3. The end time must be later than the start time.
4. The show duration must match the movie duration.
5. The service checks for overlapping active shows on the same screen.
6. The show is created.
7. A ShowSeat record is created for every active physical seat in the screen.
8. The new ShowSeats start with status Available.

A show with existing booking history cannot be moved to a different movie, screen, or time, or deactivated.

## 6. Show Seat Availability

1. The customer selects a show.
2. The customer requests its ShowSeat records.
3. The API returns the physical seat details and their current status.
4. The customer can identify seats with status Available.

Seat availability belongs to an individual show, not directly to the physical seat.

## 7. Booking Creation

1. The customer submits their name, email, and selected ShowSeat IDs.
2. The service verifies that at least one seat was selected.
3. Duplicate or invalid seat IDs are rejected.
4. A database transaction is started.
5. The service verifies that all selected seats exist and are Available.
6. All selected seats must belong to the same active, future show.
7. The server calculates the total amount using the show's ticket price.
8. A Booking record and its BookingSeat records are created.
9. The selected ShowSeats are marked Booked.
10. The transaction is committed.

If an operation fails, the transaction is rolled back.

## 8. Booking Cancellation

1. The customer submits a cancellation request for a booking.
2. The service verifies that the booking exists.
3. Only a Confirmed booking can be cancelled.
4. A database transaction is started.
5. The associated ShowSeats are changed back to Available.
6. The Booking status is changed to Cancelled.
7. The booking and booking-seat history are retained.
8. The transaction is committed.

If an operation fails, the transaction is rolled back.

## 9. Validation and Error Handling

- DTO validation checks required fields, lengths, ranges, and email formats.
- Services enforce business rules.
- Repositories handle database operations.
- Centralized exception-handling middleware returns JSON error responses.
- ArgumentException is returned as HTTP 400.
- Unexpected exceptions are returned as HTTP 500 without exposing internal exception details.

## 10. Date and Time Convention

- Show StartTime and EndTime represent Indian Standard Time (IST) for this application.
- ShowDate filtering uses the Indian local calendar date.
- Current-time comparisons for shows use local application time.
- BookingDate is stored using UTC.

The application should use a consistent server time-zone configuration when deployed.
