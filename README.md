# WebAPI Shop

A modern, scalable REST API built with ASP.NET Core and .NET 9, implementing industry best practices for API design, asynchronous programming, and clean architecture.

## Overview

WebAPI Shop is a comprehensive REST API that demonstrates professional-grade software development practices. The project is built following a three-layer architecture pattern with emphasis on separation of concerns, testability, and scalability.

## Technology Stack

- **Framework**: ASP.NET Core with .NET 9
- **Language**: C#
- **ORM**: Entity Framework Core
- **Mapping**: AutoMapper
- **Logging**: NLog
- **Testing**: XUnit with Moq
- **Architecture**: 3-Layer Architecture (Application, Services, Repositories)
- **Design Patterns**: Dependency Injection, DTO Pattern, Repository Pattern

## Project Architecture

The project follows a **three-layer architecture** pattern:

```
┌─────────────────────┐
│   Application       │ (Controllers, API Endpoints)
├─────────────────────┤
│   Services          │ (Business Logic, AutoMapper)
├─────────────────────┤
│   Repositories      │ (Data Access, Entity Framework)
└─────────────────────┘
```

### Layer Descriptions

1. **Application Layer**
   - Handles HTTP requests and responses
   - Contains API controllers and endpoints
   - Coordinates business logic execution

2. **Services Layer**
   - Contains business logic
   - Implements AutoMapper for DTO-to-Entity mapping
   - Orchestrates data operations
   - Uses Data Transfer Objects (DTOs) as records for optimal data transfer

3. **Repository Layer**
   - Manages data access using Entity Framework Core
   - Implements the Database-First approach
   - Provides asynchronous data operations for thread optimization and scalability

## Key Features

### Asynchronous Programming
- All database operations are performed asynchronously
- Improves thread utilization and application scalability
- Better resource management for high-concurrency scenarios

### Dependency Injection
- Layers communicate through dependency injection
- Provides loose coupling and improved testability
- Configured in the application startup

### DTO Layer
- Data Transfer Objects as C# records for efficient data transfer
- Eliminates circular dependencies between layers
- Clear separation between API contracts and internal models
- Bidirectional mapping with AutoMapper

### Configuration Management
- All configuration settings stored separately from code
- Configured via `appsettings.json` files
- Environment-specific configurations supported

### Logging & Error Handling
- Comprehensive logging using NLog library
- Custom `errorHandlingMiddleware` for centralized error management
- All exceptions properly logged for debugging and monitoring
- Request/response tracking and traffic monitoring

### Traffic Monitoring
- All API traffic recorded in the rating table
- Enables performance analysis and usage tracking
- Supports analytics and audit requirements

## REST Principles

The API strictly adheres to REST principles:
- Resource-based URLs
- Standard HTTP methods (GET, POST, PUT, DELETE, PATCH)
- Appropriate HTTP status codes
- Stateless communication
- JSON request/response format

## Testing

The project includes comprehensive test coverage:

### Unit Tests
- Isolated testing of individual components
- Mock dependencies using Moq
- Focus on business logic validation

### Integration Tests
- Database interaction testing
- End-to-end API flow testing
- Real-world scenario validation

### Test Frameworks
- **XUnit**: Testing framework
- **Moq**: Mocking framework
- **Moq.EntityFrameworkCore**: EF Core mocking

## Getting Started

### Prerequisites
- .NET 9 SDK or later
- Visual Studio 2022 or VS Code
- SQL Server or compatible database

### Setup Instructions

1. **Clone the repository**
   ```bash
   git clone https://github.com/yael184/WebApiShop.git
   cd WebApiShop
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure the database**
   - Updaction string in `appsettings.json`
   - Run database migrations (if applicable)

4. **Build the project**
   ```bash
   dotnet build
   ```

5. **Run the application**
   ```bash
   dotnet run --project WebApiShop
   ```

6. **Run tests**
   ```bash
   dotnet test
   ```

## Configuration

Configuration is managed through `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your_Database_Connection_String"
  },
  "NLog": {
    "logLevel": "Info"
  }
}
```

## Project Structure

```
WebApiShop/
├── WebApiShop/              # Application Layer (API Controllers)
├── Services/                # Services Layer (Business Logic)
├── Repository/              # Repository Layer (Data Access)
├── Entities/                # Entity Framework Models
├── DTOs/                    # Data Transfer Objects
├── Tests/                   # Unit & Integration Tests
└── Program.cs               # Application Entry Point
```

## API Documentation

The API endpoints follow standard REST conventions:

- `GET /api/resource` - Retrieve all resources
- `GET /api/resource/{id}` - Retrieve a specific resource
- `POST /api/resource` - Create a new resource
- `PUT /api/resource/{id}` - Update a resource
- `DELETE /api/resource/{id}` - Delete a resource

## Error Handling

The application uses centralized error handling through the `errorHandlingMiddleware`. All exceptions are:
- Caught and logged using NLog
- Transformed into appropriate HTTP responses
- Tracked for monitoring and debugging

## Scalability Considerations

- Asynchronous operations for efficient thread management
- Dependency injection for loose coupling and testing
- Stateless API design
- Traffic monitoring for performance analysis
- Clean architecture for easier maintenance and scaling

## Database Approach

The project uses **Database-First** approach with Entity Framework Core:
- Database schema drives the model generation
- Entity models automatically generated from database
- Asynchronous database access patterns
- ORM abstraction layer for data access

## Contributing

When contributing to this project, please ensure:
- Code follows the existing style and conventions
- Unit and integration tests are included for new features
- All tests pass before submission
- Logging is implemented for important operations
- DTOs are used for API contracts


