# Movie Ticket Booking System – Architecture

## 1. Architecture Overview

The application follows a layered architecture using ASP.NET Core Web API and Entity Framework Core.

The request flow is:

**Client → Controller → Service → Repository → DbContext → SQL Server**

## 2. Application Layers

### Controllers
Receive HTTP requests, call services, and return appropriate HTTP responses.

### Services
Implement business logic, validations, and application workflows.

### Repositories
Handle database access and data retrieval operations.

### Data
Contains the Entity Framework Core DbContext and database configuration.

### Models
Represent the database entities.

### DTOs
Define request and response data structures exchanged through the API.

### Middleware
Handles common HTTP request processing, including global exception handling.

## 3. Technology Stack

- C#
- .NET 10
- ASP.NET Core Web API
- Entity Framework Core 10
- SQL Server
- Visual Studio Code
- Postman

## 4. Design Principles

- Separation of concerns
- Dependency injection
- Asynchronous database operations
- DTO-based API communication
- Business logic maintained in services
- Database operations maintained in repositories

## 5. Project Scope

Version 1 focuses on movie management, theatre and screen management, seat management, show scheduling, seat availability, booking, and cancellation.
