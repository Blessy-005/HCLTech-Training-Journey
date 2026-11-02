# Movie Ticket Booking System - Architecture

## 1. Architecture Overview

The application follows a layered architecture using ASP.NET Core Web API and Entity Framework Core.

The request flow is:

**Client -> Controller -> Service -> Repository -> DbContext -> SQL Server**

## 2. Application Layers

### Controllers
Receive HTTP requests, call the appropriate services, and return HTTP responses.

### Services
Implement business logic, input validation, and application workflows.

### Repositories
Handle database queries, entity retrieval, insertion, updating, and deletion.

### Data
Contains ApplicationDbContext, which manages entity configurations and database relationships.

### Models
Represent database entities such as Movie, Theatre, Screen, Seat, Show, ShowSeat, Booking, and BookingSeat.

### DTOs
Define request and response structures exchanged through API endpoints. They prevent direct exposure of database entities.

### Middleware
Centralized exception-handling middleware converts application exceptions into consistent HTTP error responses.

## 3. Dependency Injection

Repository and service interfaces are registered in Program.cs.

ASP.NET Core dependency injection provides the required implementations to controllers and services.

## 4. Technology Stack

- C#
- .NET 10
- ASP.NET Core Web API
- Entity Framework Core 10
- SQL Server Express
- Visual Studio
- Postman
- Git and GitHub

## 5. Design Principles

- Separation of concerns
- Dependency injection
- Asynchronous database operations
- DTO-based API communication
- Business logic maintained in services
- Database operations maintained in repositories
- Database transactions for booking workflows
- Soft deletion for selected master records

## 6. Project Modules

- Movies
- Theatres
- Screens
- Seats
- Shows
- Show Seats
- Bookings

## 7. Project Scope

Version 1 focuses on movie management, theatre and screen management, seat management, show scheduling, show-specific seat availability, booking, and cancellation.

Authentication, online payments, temporary seat holds, notifications, and frontend development are outside Version 1.

## Authentication and Authorization Architecture

The application uses ASP.NET Core Identity and JWT Bearer Authentication.

Authentication flow:
1. A user registers or logs in through AuthController.
2. AuthService validates the credentials using UserManager and SignInManager.
3. A JWT containing user ID, name, email and role claims is generated.
4. The client sends the token in the Authorization Bearer header.
5. JWT middleware validates the token.
6. Authorization policies and role attributes control access to protected endpoints.

Booking ownership:
- BookingsController obtains the authenticated user's ID from JWT claims.
- BookingService checks ownership before retrieving or cancelling a booking.
- Admin users can access bookings across customers.
