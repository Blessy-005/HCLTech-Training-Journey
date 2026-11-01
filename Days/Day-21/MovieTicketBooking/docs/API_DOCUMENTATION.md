# Movie Ticket Booking System - API Documentation

## 1. Base URL

Local development URL:

https://localhost:7231

All endpoints use the `/api` prefix.

## 2. Movies API

Base route: `/api/movies`

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/movies` | Retrieve movies with optional filters and pagination |
| GET | `/api/movies/{id}` | Retrieve a movie by ID |
| POST | `/api/movies` | Create a movie |
| PUT | `/api/movies/{id}` | Update a movie |
| DELETE | `/api/movies/{id}` | Deactivate a movie |

Supported query parameters: `search`, `language`, `genre`, `pageNumber`, `pageSize`.

## 3. Theatres API

Base route: `/api/theatres`

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/theatres` | Retrieve theatres with optional filters and pagination |
| GET | `/api/theatres/{id}` | Retrieve a theatre by ID |
| POST | `/api/theatres` | Create a theatre |
| PUT | `/api/theatres/{id}` | Update a theatre |
| DELETE | `/api/theatres/{id}` | Deactivate a theatre |

Supported query parameters: `search`, `city`, `pageNumber`, `pageSize`.

## 4. Screens API

Base route: `/api/screens`

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/screens` | Retrieve screens with optional filters and pagination |
| GET | `/api/screens/{id}` | Retrieve a screen by ID |
| POST | `/api/screens` | Create a screen |
| PUT | `/api/screens/{id}` | Update a screen |
| DELETE | `/api/screens/{id}` | Deactivate a screen |

Supported query parameters: `search`, `theatreId`, `pageNumber`, `pageSize`.

## 5. Seats API

Base route: `/api/seats`

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/seats` | Retrieve seats with optional filters and pagination |
| GET | `/api/seats/{id}` | Retrieve a seat by ID |
| POST | `/api/seats` | Create a seat |
| POST | `/api/seats/bulk` | Create multiple seats |
| PUT | `/api/seats/{id}` | Update a seat |
| DELETE | `/api/seats/{id}` | Deactivate a seat |

Supported query parameters: `search`, `screenId`, `seatType`, `pageNumber`, `pageSize`.

## 6. Shows API

Base route: `/api/shows`

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/shows` | Retrieve shows with optional filters and pagination |
| GET | `/api/shows/{id}` | Retrieve a show by ID |
| POST | `/api/shows` | Create a show |
| PUT | `/api/shows/{id}` | Update a show |
| DELETE | `/api/shows/{id}` | Deactivate a show |

Supported query parameters: `movieId`, `screenId`, `showDate`, `isActive`, `pageNumber`, `pageSize`.

Show creation validates movie and screen availability, movie duration, and overlapping schedules on the same screen.

## 7. Show Seats API

Base route: `/api/showseats`

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/showseats` | Retrieve seat availability for a show |

Required query parameter: `showId`.

Optional query parameter: `status`.

Example:

`GET /api/showseats?showId=1&status=Available`

## 8. Bookings API

Base route: `/api/bookings`

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/bookings` | Retrieve bookings with optional filters and pagination |
| GET | `/api/bookings/{id}` | Retrieve a booking by ID |
| POST | `/api/bookings` | Create a booking |
| PUT | `/api/bookings/{id}/cancel` | Cancel a confirmed booking |

Supported query parameters: `customerEmail`, `status`, `bookingDate`, `pageNumber`, `pageSize`.

## 9. HTTP Status Codes

| Status Code | Meaning |
|---|---|
| 200 OK | Request completed successfully |
| 201 Created | Resource created successfully |
| 204 No Content | Request completed without a response body |
| 400 Bad Request | Invalid input or business rule violation |
| 404 Not Found | Requested resource not found |
| 500 Internal Server Error | Unexpected server error |

Actual status codes depend on the controller action and exception handling.

## 10. API Testing

All endpoints will be verified using Postman.

The testing process includes successful requests, invalid inputs, missing resources, filtering, pagination, booking creation, and cancellation.
