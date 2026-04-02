# Digital Wallet and P2P Transfer Service

A backend service for managing digital wallets and peer-to-peer money transfers. Built with .NET 10 Minimal API, PostgreSQL, and RabbitMQ for asynchronous transfer processing.

## Why This Project

This project was built as a hands-on learning exercise to understand how production-grade backend systems work end to end. The goal was not just to build CRUD endpoints, but to tackle real challenges found in financial systems:

- **Asynchronous processing**: Transfers are not processed inline. They are published to a message queue and handled by a separate worker service, simulating how real payment systems decouple request acceptance from processing.
- **Idempotency**: Every transfer request carries an idempotency key to prevent double-spend scenarios where the same request is accidentally submitted twice.
- **Event-driven architecture**: A single transfer triggers multiple downstream actions (balance updates, notifications, audit logging), each handled by independent consumers.
- **Clean Architecture**: The codebase enforces strict dependency direction. Domain has zero external dependencies. Application defines interfaces. Infrastructure implements them. API wires everything together.

## Architecture

```
Client --> Minimal API (JWT Auth) --> PostgreSQL
                |
                v
            RabbitMQ
                |
                v
        Background Worker
        ├── TransferProcessor (debit/credit in DB transaction)
        ├── NotificationConsumer (simulated notifications)
        └── AuditLogConsumer (writes audit trail)
```

**How a transfer works:**

1. The API validates the request and creates a `TransferRequest` with status `Pending`
2. A `TransferRequestedEvent` is published to RabbitMQ
3. The API returns immediately with the pending status
4. The Worker picks up the event and processes it inside a database transaction:
   - Debits the sender wallet
   - Credits the receiver wallet
   - Creates transaction records for both parties
   - Marks the transfer as `Completed` (or `Failed` if something goes wrong)
5. Downstream consumers handle notifications and audit logging

## Tech Stack

| Component | Technology |
|---|---|
| Framework | .NET 10 Minimal API |
| Database | PostgreSQL 17 with EF Core 10 |
| Messaging | RabbitMQ with MassTransit 8.3 |
| Authentication | JWT Bearer tokens (BCrypt password hashing) |
| Validation | FluentValidation |
| Testing | xUnit, NSubstitute |
| Containerization | Docker, Docker Compose |
| CI/CD | GitHub Actions |
| Code Analysis | SonarAnalyzer |

## Project Structure

```
src/
├── DigitalWallet.Domain           # Entities, enums, repository interfaces
├── DigitalWallet.Application      # Services, DTOs, validators, events, interfaces
├── DigitalWallet.Infrastructure   # EF Core, repositories, JWT, messaging consumers
├── DigitalWallet.Api              # Endpoints, filters, DI configuration
└── DigitalWallet.Worker           # Background service for processing transfers

tests/
└── DigitalWallet.UnitTests        # Entity, validator, and service tests
```

Dependency flow: `Domain <-- Application <-- Infrastructure <-- Api/Worker`

Domain references nothing. Each layer only depends on the layers to its left.

## API Endpoints

### Authentication

| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/api/auth/register` | No | Register a new user with a wallet |
| POST | `/api/auth/login` | No | Login and receive a JWT token |

### Wallets

| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/api/wallets/me` | Yes | Get current user's wallet |
| POST | `/api/wallets/deposit` | Yes | Deposit funds into wallet |

### Transfers

| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/api/transfers` | Yes | Create a P2P transfer |
| GET | `/api/transfers/{id}` | Yes | Get transfer status by ID |

### Transactions

| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/api/transactions` | Yes | Get paginated transaction history |

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/products/docker-desktop)

### Run with Docker Compose

```bash
docker compose up --build
```

This starts all four services:

| Service | Port |
|---|---|
| API | http://localhost:5000 |
| PostgreSQL | localhost:5432 |
| RabbitMQ | localhost:5672 |
| RabbitMQ Management UI | http://localhost:15672 (guest/guest) |

### Run Locally (without Docker)

1. Start PostgreSQL and RabbitMQ on your machine

2. Update connection strings in `src/DigitalWallet.Api/appsettings.Development.json`

3. Apply migrations:
```bash
dotnet ef database update --project src/DigitalWallet.Infrastructure --startup-project src/DigitalWallet.Api
```

4. Run the API and Worker in separate terminals:
```bash
dotnet run --project src/DigitalWallet.Api
dotnet run --project src/DigitalWallet.Worker
```

### Run Tests

```bash
dotnet test tests/DigitalWallet.UnitTests
```

## Configuration

The API requires the following configuration (set via environment variables or appsettings):

| Key | Description |
|---|---|
| `ConnectionStrings:DefaultConnection` | PostgreSQL connection string |
| `RabbitMQ:Host` | RabbitMQ hostname |
| `JwtSettings:Secret` | JWT signing key (minimum 32 characters) |
| `JwtSettings:Issuer` | Token issuer |
| `JwtSettings:Audience` | Token audience |
| `JwtSettings:ExpiryMinutes` | Token expiration time in minutes |
