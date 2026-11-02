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
## 15. Actual Postman Test Results

**Testing Date:** November 1, 2026
**Environment:** Local ASP.NET Core Web API, .NET 10, SQL Server Express
**Testing Tool:** Postman
**Base URL:** https://localhost:7231

### 15.1 Movies Module

| Test | HTTP Status | Result |
|---|---:|---|
| Get all movies | 200 | Passed |
| Get movie by ID | 200 | Passed |
| Create movie | 201 | Passed |
| Update movie | 204 | Passed |
| Verify updated movie | 200 | Passed |
| Delete movie (soft delete) | 204 | Passed |
| Retrieve inactive movie | 404 | Passed |

### 15.2 Theatres Module

| Test | HTTP Status | Result |
|---|---:|---|
| Get all theatres | 200 | Passed |
| Create theatre | 201 | Passed |
| Get theatre by ID | 200 | Passed |
| Update theatre | 204 | Passed |
| Verify updated theatre | 200 | Passed |
| Delete theatre (soft delete) | 204 | Passed |
| Retrieve inactive theatre | 404 | Passed |
| Create replacement active theatre | 201 | Passed |

### 15.3 Screens Module

| Test | HTTP Status | Result |
|---|---:|---|
| Get all screens | 200 | Passed |
| Create screen | 201 | Passed |
| Get screen by ID | 200 | Passed |
| Update screen | 204 | Passed |
| Verify updated screen | 200 | Passed |
| Delete screen (soft delete) | 204 | Passed |
| Retrieve inactive screen | 404 | Passed |
| Create replacement active screen | 201 | Passed |

### 15.4 Seats Module

| Test | HTTP Status | Result |
|---|---:|---|
| Get all seats | 200 | Passed |
| Create individual seat | 201 | Passed |
| Bulk create seats | 200 | Passed |
| Reject duplicate seat | 400 | Passed |
| Get seat by ID | 200 | Passed |
| Update seat | 204 | Passed |
| Verify updated seat | 200 | Passed |
| Delete seat (soft delete) | 204 | Passed |
| Retrieve inactive seat | 404 | Passed |

### 15.5 Shows Module

| Test | HTTP Status | Result |
|---|---:|---|
| Get all shows | 200 | Passed |
| Create show | 201 | Passed |
| Get show by ID | 200 | Passed |
| Reject overlapping show | 400 | Passed |
| Update show | 200 | Passed |
| Verify updated ticket price | 200 | Passed |

### 15.6 ShowSeats Module

| Test | HTTP Status | Result |
|---|---:|---|
| Get seats for a show | 200 | Passed |
| Filter seats by Available status | 200 | Passed |
| Verify automatic ShowSeat generation | 200 | Passed |
| Verify booked seat status | 200 | Passed |
| Verify released seat status | 200 | Passed |
| Verify seat status after rebooking | 200 | Passed |

### 15.7 Bookings Module

| Test | HTTP Status | Result |
|---|---:|---|
| Get all bookings | 200 | Passed |
| Create booking with two seats | 201 | Passed |
| Get booking by ID | 200 | Passed |
| Verify server-calculated total amount | 201 | Passed |
| Reject duplicate ShowSeat IDs | 400 | Passed |
| Reject already-booked seats | 400 | Passed |
| Reject invalid ShowSeat ID | 400 | Passed |
| Reject invalid email address | 400 | Passed |
| Cancel confirmed booking | Successful | Passed |
| Verify released seats | 200 | Passed |
| Verify cancelled booking history | 200 | Passed |
| Reject repeated cancellation | 400 | Passed |
| Rebook a previously released seat | 201 | Passed |
| Verify final seat availability | 200 | Passed |

### 15.8 End-to-End Booking Scenario

The following workflow was successfully tested:

1. Created a movie, theatre, screen, and physical seats.
2. Created a future show for Interstellar.
3. Verified automatic generation of four ShowSeat records.
4. Retrieved available seats for the show.
5. Created Booking ID 1 for two seats.
6. Verified the total amount of ₹440 (2 × ₹220).
7. Confirmed that both selected seats changed to Booked.
8. Cancelled Booking ID 1.
9. Confirmed that both seats returned to Available.
10. Verified that the cancelled booking history remained accessible.
11. Successfully created Booking ID 2 using a previously released seat.
12. Confirmed that the rebooked seat changed back to Booked.

### 15.9 Testing Notes

- All recorded Postman tests returned the expected successful or validation status codes.
- Booking totals were calculated by the server using the show's ticket price.
- Booking cancellation released seats while preserving booking history.
- A previously released seat could be booked again.
- The update-show endpoint returned HTTP 200 with a success message rather than HTTP 204.
- Tests not explicitly recorded in this section remain part of the planned testing checklist and should not be considered verified.

## Authentication and Authorization Test Results

The following scenarios were tested using Postman:

| Scenario | Expected Result | Result |
|---|---|---|
| Admin login | 200 OK | Passed |
| Customer registration | 200 OK | Passed |
| Customer login | 200 OK | Passed |
| Admin creates a movie | 201 Created | Passed |
| Customer attempts Admin-only movie creation | 403 Forbidden | Passed |
| Unauthenticated booking access | 401 Unauthorized | Passed |
| Customer accesses own booking | 200 OK | Passed |
| Customer accesses another customer's booking | 404 Not Found | Passed |
| Customer cancels another customer's booking | 404 Not Found | Passed |
| Booking owner cancels own booking | 200 OK | Passed |
| Seat status after cancellation | Available | Passed |

### Areas Where Difficulties Were Faced

1. Integrating ASP.NET Core Identity with the existing database.
2. Configuring JWT token generation and validation.
3. Implementing role-based authorization for Admin and Customer.
4. Managing booking ownership and preventing unauthorized access.
5. Handling EF Core migrations and database relationships.
6. Testing authentication, authorization and booking cancellation using Postman.
