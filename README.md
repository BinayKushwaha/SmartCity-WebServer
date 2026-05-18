# SmartCity

A modular .NET 8 solution for managing real estate commission rates and retail property listings, featuring robust caching, repository patterns, and comprehensive unit testing.

## Features

- **Commission Calculation:** Dynamically calculates commission based on configurable slabs.
- **Caching:** Utilizes in-memory caching for performance, with configurable expiry.
- **Repository Pattern:** Clean separation of data access logic.
- **Unit Testing:** NUnit and Moq-based tests for core business logic.
- **Extensible DTOs:** Clear data transfer objects for API and service layers.

## Technologies

- **.NET 8 / C# 12**
- **NUnit** for unit testing
- **Moq** for mocking dependencies
- **Microsoft.Extensions.Options** for configuration

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- Visual Studio 2022 or later

### Build & Run

1. Clone the repository:
    ```sh
    git clone <your-repo-url>
    cd SmartCity
    ```

2. Restore dependencies:
    ```sh
    dotnet restore
    ```

3. Build the solution:
    ```sh
    dotnet build
    ```

4. Run tests:
    ```sh
    dotnet test
    ```

### Project Structure

- `SmartCity.Application` – Core business logic, services, DTOs, interfaces
- `SmartCity.Domain` – Entity models
- `SmartCity.Infrastructure` – Data access, caching, configuration
- `SmartCity-WebServer` – API layer (controllers, startup)
- `SmartCity.NTest` – Unit tests

## Example: Commission Calculation

The `CommissionService` calculates commission based on price and active slabs. Example test:
