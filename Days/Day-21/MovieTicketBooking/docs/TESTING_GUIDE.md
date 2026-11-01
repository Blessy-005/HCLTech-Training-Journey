# Movie Ticket Booking System - Testing Guide

## 1. Testing Environment

- API: ASP.NET Core Web API
- Framework: .NET 10
- Database: SQL Server Express
- Database name: MovieTicketBookingDb
- API testing tool: Postman
- Local base URL: https://localhost:7231

Start the API from Visual Studio before sending requests.

## 2. Testing Strategy

Test each module independently before testing the complete booking workflow.

For every endpoint, verify:
- HTTP status code.
- Response body and returned data.
- Input validation.
- Database changes.
- Correct handling of missing or invalid resources.

## 3. Movies API Tests

- Retrieve all movies.
- Retrieve a movie by ID.
- Create a movie with valid data.
- Create a movie with missing or invalid fields.
- Update an existing movie.
- Retrieve movies using search, language, and genre filters.
- Verify pagination.
- Deactivate a movie.
- Verify that inactive movies are excluded from normal listings.

## 4. Theatres API Tests

- Retrieve all theatres.
- Retrieve a theatre by ID.
- Create a theatre.
- Test missing and invalid fields.
- Update a theatre.
- Filter theatres by name and city.
- Verify pagination.
- Deactivate a theatre.
- Verify inactive theatres are excluded from normal listings.

## 5. Screens API Tests

- Retrieve all screens.
- Retrieve a screen by ID.
- Create a screen using an active theatre.
- Try creating a screen with an invalid or inactive theatre.
- Update a screen.
- Filter screens by theatre and name.
- Verify pagination.
- Deactivate a screen.

## 6. Seats API Tests

- Retrieve all seats.
- Retrieve a seat by ID.
- Create an individual seat.
- Create multiple seats using the bulk endpoint.
- Test an empty bulk request.
- Test duplicate seat numbers in the same screen.
- Test seat-number normalization and case handling.
- Filter seats by screen and seat type.
- Update a seat that has not been assigned to a show.
- Verify that a seat assigned to a show cannot be modified.
- Verify that a seat assigned to a show cannot be deactivated.

## 7. Shows API Tests

- Retrieve all shows.
- Retrieve a show by ID.
- Create a show with valid movie and screen details.
- Test invalid movie and screen IDs.
- Test an end time earlier than or equal to the start time.
- Test a duration that does not match the movie.
- Test overlapping shows on the same screen.
- Test non-overlapping shows on the same screen.
- Verify that ShowSeat records are generated for active seats.
- Update an eligible show.
- Verify restrictions on modifying shows with booking history.
- Verify show deactivation rules.

## 8. Show Seats API Tests

- Retrieve ShowSeats for a valid show.
- Filter ShowSeats by Available status.
- Verify that each physical seat appears only once per show.
- Verify that newly generated ShowSeats have Available status.
- Verify that booked seats are displayed with Booked status.

## 9. Booking API Tests

- Retrieve all bookings.
- Retrieve a booking by ID.
- Create a booking with one available seat.
- Create a booking with multiple available seats.
- Try creating a booking without selected seats.
- Try submitting duplicate ShowSeat IDs.
- Try submitting invalid ShowSeat IDs.
- Try booking seats from different shows.
- Try booking already booked seats.
- Try booking seats for an inactive or past show.
- Verify that the server calculates the total amount.
- Verify that selected ShowSeats become Booked.
- Verify that booking and BookingSeat records are saved.

## 10. Booking Cancellation Tests

- Cancel a confirmed booking.
- Verify that the booking status becomes Cancelled.
- Verify that its ShowSeats become Available.
- Verify that booking history remains in the database.
- Try cancelling an already cancelled booking.
- Try cancelling a nonexistent booking.

## 11. Error Response Tests

Verify:
- Invalid request bodies.
- Missing required fields.
- Invalid numeric ranges.
- Invalid email addresses.
- Nonexistent resource IDs.
- Business-rule violations.
- Unexpected server errors return a generic error response without exposing internal exception details.

## 12. SQL Server Verification

After important operations, inspect the database to verify:

- Movie, theatre, screen, and seat records.
- Show and ShowSeat records.
- Booking and BookingSeat records.
- Seat status changes after booking.
- Seat status changes after cancellation.
- Booking history is retained after cancellation.

## 13. End-to-End Booking Scenario

Perform the following complete workflow:

1. Create a movie.
2. Create a theatre.
3. Create a screen for the theatre.
4. Create physical seats for the screen.
5. Create a future show with a matching movie duration.
6. Retrieve the show's available seats.
7. Create a booking using available ShowSeat IDs.
8. Verify the booking and booked seat statuses.
9. Retrieve the booking.
10. Cancel the booking.
11. Verify the cancelled booking and released seats.
12. Confirm the database records and relationships.

## 14. Final Verification

- Run dotnet build.
- Confirm there are no build errors.
- Execute all Postman test cases.
- Verify SQL Server data.
- Review API responses and error handling.
- Update this guide with actual test results.
- Commit final documentation and testing changes.
- Push the completed local commits to GitHub.
