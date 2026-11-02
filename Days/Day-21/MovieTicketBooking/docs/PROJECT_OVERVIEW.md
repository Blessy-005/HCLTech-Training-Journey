# Movie Ticket Booking System

## 1. Project Overview

The Movie Ticket Booking System is a backend REST API application developed using ASP.NET Core Web API and Entity Framework Core.

It allows users to manage movies, theatres, screens, seats, and show schedules. Customers can view seat availability, book tickets, retrieve their bookings, and cancel confirmed bookings.

The application uses SQL Server Express for persistent data storage.

## 2. Objectives

- Manage movie information.
- Manage theatres and their screens.
- Manage seats within each screen.
- Schedule movie shows.
- Maintain seat availability for each show.
- Allow customers to book available seats.
- Calculate booking amounts on the server.
- Support booking retrieval and cancellation.
- Maintain booking history when a booking is cancelled.

## 3. Technology Stack

| Technology | Purpose |
|---|---|
| C# | Backend programming language |
| .NET 10 | Application platform |
| ASP.NET Core Web API | REST API development |
| Entity Framework Core 10 | Object-relational mapping |
| SQL Server Express | Relational database |
| Visual Studio | Development environment |
| Postman | API testing |
| Git and GitHub | Version control |

## 4. Main Modules

1. **Movies** - Create, retrieve, update, and deactivate movies.
2. **Theatres** - Manage theatre information.
3. **Screens** - Manage screens belonging to theatres.
4. **Seats** - Manage physical seats belonging to screens, including bulk creation.
5. **Shows** - Schedule shows and validate scheduling conflicts.
6. **Show Seats** - Track seat availability separately for each show.
7. **Bookings** - Create bookings, retrieve booking details, and cancel confirmed bookings.

## 5. Booking Workflow

1. A customer selects a scheduled show.
2. The customer checks available seats.
3. The customer submits the selected seat IDs and customer details.
4. The API validates seat availability and selection.
5. The server calculates the total booking amount.
6. The booking and booking-seat records are saved.
7. The selected show seats are marked as booked.
8. When a confirmed booking is cancelled, its seats are released while the booking history is retained.

Booking creation and cancellation use database transactions.

## 6. Version 1 Scope

Version 1 includes:

- Movie, theatre, screen, and seat management.
- Show scheduling and overlap validation.
- Show-specific seat availability.
- Booking creation and retrieval.
- Booking cancellation.
- Input validation and centralized exception handling.
- SQL Server database integration.

The following are outside Version 1:

- User authentication and authorization.
- Online payment integration.
- Temporary seat reservations or seat holds.
- Email and SMS notifications.
- Frontend application.
- Production deployment automation.

## 7. Architecture

The application follows a layered architecture:

**Controller -> Service -> Repository -> DbContext -> SQL Server**

Each layer has a separate responsibility. Business rules are implemented in services, while repositories handle database operations.

## 8. Project Status

Backend modules have been implemented. Final API verification using Postman and documentation validation are part of the project completion process.

## Authentication and Authorization

- Integrated ASP.NET Core Identity for user management.
- Implemented JWT-based registration and login.
- Added Admin and Customer roles.
- Restricted administrative operations to Admin users.
- Implemented customer-specific booking access and ownership protection.
- Used ASP.NET Core User Secrets for the JWT signing key.
