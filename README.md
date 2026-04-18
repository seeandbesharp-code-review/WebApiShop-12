# ASP.NET Core REST API

This is a modern REST API project built using .NET 9, implementing clean architecture and industry best practices for performance, scalability, and maintainability.

---

## 🚀 Core Technologies

* **Framework:** ASP.NET Core (.NET 9)
* **Language:** C# (Asynchronous implementation)
* **ORM:** Entity Framework Core (Database First)
* **Mapping:** AutoMapper
* **Logging:** nLog
* **Testing:** Unit Tests & Integration Tests

---

## 🏗️ Architecture & Design Patterns

The project is built using a **3-Layer Architecture** to ensure a clear separation of concerns:

* **Application Layer:** Handles Controllers, Middlewares, and incoming HTTP requests.
* **Services Layer:** Contains the core business logic.
* **Repositories Layer:** Manages data access and communication with the database.

### Key Principles

* **Dependency Injection (DI):**
  Used throughout the system to achieve decoupling between layers, ensuring the code is flexible and easily testable.

* **Asynchronous Programming:**
  All operations (especially DB access) are implemented asynchronously (`async/await`) to free up the request thread and enhance system scalability.

* **Data Transfer Objects (DTO):**

  * Utilized C# Records for DTOs to ensure immutability and efficient data transfer.
  * Implemented AutoMapper to handle conversions between Entities and DTOs, preventing circular dependencies.

---

## 🛠️ Infrastructure & Monitoring

* **Configuration Management:**
  Application settings are stored separately from the code within `appsettings.json` files for environment flexibility.

* **Global Error Handling:**
  A custom `ErrorHandlingMiddleware` manages all system exceptions centrally, providing consistent error responses and simplified monitoring.

* **Logging:**
  Integrated nLog for comprehensive logging of application behavior and errors.

* **Traffic Tracking:**
  All system traffic and ratings are recorded in a dedicated `Rating` table for auditing and analysis.

---

## 🧪 Quality Assurance

To ensure code reliability and robustness, the project includes:

* **Unit Tests:** Testing individual components and business logic in isolation.
* **Integration Tests:** Testing the end-to-end flow and interaction between the different layers.

---

## ⚙️ Setup and Installation

1. Clone the repository.

2. Configure your connection string in the `appsettings.json` file.

3. Restore dependencies:

```bash
dotnet restore
```

4. Run the application:

```bash
dotnet run
```
