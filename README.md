# Flight Booking System ✈️

A flight booking system built with **Clean Architecture**, **CQRS**, and optimized caching strategies.

---

## 🛠️ Tech Stack

- **.NET 8.0** - ASP.NET Core Web API
- **MediatR** - CQRS pattern
- **FluentValidation** - Request validation
- **Entity Framework Core** - ORM
- **SQL Server** - Database
- **Redis** - Distributed cache
- **Docker** - Containerization
- **Blazor Server** - UI (optional)

---

## 🚀 Quick Start

### 1. Start Infrastructure
```bash
docker-compose up -d
```
This starts Redis and SQL Server.

### 2. Run API
```bash
cd FlightBooking.API
dotnet run
```
- API: http://localhost:5259
- Swagger: http://localhost:5259/swagger

### 3. Run UI (Optional)
```bash
cd FlightBooking.Web
dotnet run
```
- Web UI: https://localhost:7002

---

## 🔌 API Endpoints

### Search Flights
```
GET /Flights/search?origin=IST&destination=AMS&departureDate=2026-02-20
```

**Response:**
```json
[
  {
    "id": "TK123_202602201430",
    "flightNumber": "TK123",
    "origin": "IST",
    "destination": "AMS",
    "departureTime": "2026-02-20T14:30:00",
    "arrivalTime": "2026-02-20T17:45:00",
    "price": 450.00,
    "duration": "03:15:00"
  }
]
```

### Get Flight Details
```
GET /Flights/{flightId}
```

### Get Airports
```
GET /Airports
```

---

## 🏗️ Architecture

```
FlightBooking.API          → REST API
FlightBooking.Application  → CQRS Handlers, Validation
FlightBooking.Infrastructure → EF Core, Redis, Providers
FlightBooking.Domain       → Entities
FlightBooking.Web          → Blazor UI
```

**Patterns:**
- Clean Architecture
- CQRS (MediatR)
- Repository Pattern
- Dependency Injection

---

## ⚙️ Configuration

Edit `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=FlightBooking;User Id=sa;Password=YourStrong!Pass123;TrustServerCertificate=True;"
  },
  "Redis": {
    "Configuration": "localhost:6379"
  }
}
```

---

## 📦 Features

- ✅ Flight search with caching (10 min TTL)
- ✅ Individual flight cache
- ✅ Airport autocomplete
- ✅ Automatic database migrations
- ✅ Request validation (FluentValidation)
- ✅ CORS enabled for Web UI
- ✅ Swagger documentation

---

## 🐛 Troubleshooting

**Redis connection issue:**
```bash
docker ps | grep redis
docker exec -it flight-redis redis-cli ping
```

**Migration failed:**
```bash
dotnet ef database update --project FlightBooking.Infrastructure --startup-project FlightBooking.API
```

**Port already in use:**
```bash
lsof -i :5259
kill -9 <PID>
```

---

## 📝 Useful Commands

```bash
# Start infrastructure
docker-compose up -d

# Run API
cd FlightBooking.API && dotnet run

# Stop infrastructure
docker-compose down

# Add migration
dotnet ef migrations add MigrationName --project FlightBooking.Infrastructure --startup-project FlightBooking.API

# Build solution
dotnet build
```

---

## 📚 Project Structure

```
FlightBooking/
├── FlightBooking.API/              # REST API
├── FlightBooking.Application/      # CQRS, DTOs, Validators
├── FlightBooking.Infrastructure/   # EF Core, Redis, Providers
├── FlightBooking.Domain/           # Entities
└── FlightBooking.Web/              # Blazor UI
```

---

**Built with .NET 8.0 & Clean Architecture**
