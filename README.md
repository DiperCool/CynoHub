# CynoHub API

CynoHub is a backend application built for dog breeders to manage and publish their litters. The project is designed with a strict adherence to Clean Architecture principles, ensuring separation of concerns, high testability, and a domain-centric approach.

## Architecture

The solution follows Clean Architecture patterns:

- **CynoHub.Domain**: The core of the system. Contains business entities (`Litter`, `BreederBenefit`), enums, repository interfaces, and domain exceptions. It has no external dependencies.
- **CynoHub.Application**: The use case layer. Orchestrates business logic, validates constraints, and interacts with repositories. It depends only on the Domain layer.
- **CynoHub.Infrastructure**: The persistence and external services layer. Implements Entity Framework Core repositories, Unit of Work, database configurations, and infrastructure-specific services (like dummy email notifications).
- **CynoHub.Api**: The presentation layer. Contains ASP.NET Core controllers, global exception handling middleware, and Swagger documentation.
- **CynoHub.UnitTests**: Contains unit tests for the Application and Domain layers, heavily utilizing `Moq` and `xUnit`.

## Key Features

- **Optimistic Concurrency**: Uses EF Core's concurrency tokens (Version) to prevent race conditions during high-load operations, such as consuming a limited publication slot.
- **Domain Exceptions**: Centralized exception handling via custom middleware that maps domain exceptions to appropriate HTTP status codes (e.g., 403 Forbidden, 404 Not Found, 409 Conflict, 422 Unprocessable Entity).
- **Unit of Work Pattern**: Ensures atomic database transactions across multiple repository operations.
- **Automatic Seed Data**: Populates a SQLite database with initial test data (Breeders, Benefits, and Litters) on the first run in a Development environment.

## Prerequisites

- .NET 8 SDK
- An IDE or text editor of your choice (Rider, Visual Studio, VS Code)

## Getting Started

1. **Clone the repository and navigate to the root directory.**

2. **Run the Backend:**
   ```bash
   cd backend
   dotnet test
   cd src/CynoHub.Api
   dotnet run
   ```
   *(Backend Swagger will be available at `https://localhost:7052/swagger`)*

3. **Run the Frontend:**
   ```bash
   cd frontend
   npm install
   npm run dev
   ```
   *(Frontend UI will be available at `http://localhost:3000`)*

## Testing the Limits Logic

By default, the seed data creates a test breeder with a limit of 3 free publication slots. It also creates 4 litters with the `Approved` status. 

You can test the API by calling the `POST /api/litters/{litterId}/publish` endpoint via Swagger:
1. Provide the header `X-Breeder-Id: 00000000-0000-0000-0000-000000000001`.
2. Provide the `litterId` of an approved litter (e.g., `00000000-0000-0000-0000-000000000010`).
3. After publishing 3 litters successfully, any subsequent attempts to publish a 4th litter will result in a `422 Unprocessable Entity` due to exhausted limits.

## Project TODOs

Future improvements and technical debt are tracked in the `TODO.md` file located in the root directory.
