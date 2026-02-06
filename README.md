# POS-WMS Integration System

Enterprise-grade Point of Sale (POS) to Warehouse Management System (WMS) integration built with .NET 10, event-driven architecture, and modern best practices.

## 🏗️ Architecture

```
┌──────────────────┐     ┌─────────────────┐     ┌──────────────────┐
│                  │     │                 │     │                  │
│     POS API      │────▶│  Apache Kafka   │────▶│   WMS Consumer   │
│                  │     │                 │     │                  │
└────────┬─────────┘     └─────────────────┘     └────────┬─────────┘
         │                                                │
         ▼                                                ▼
┌──────────────────┐                            ┌──────────────────┐
│   POS Database   │                            │   WMS Database   │
│   (PostgreSQL)   │                            │   (PostgreSQL)   │
└──────────────────┘                            └──────────────────┘
```

## 📋 Features

### POS Service
- **Sales Management**: Create, complete, and cancel sales transactions
- **Returns Processing**: Handle product returns with inventory restoration
- **Payment Processing**: Multi-payment method support
- **CQRS Pattern**: Clean separation of commands and queries
- **Domain-Driven Design**: Rich domain model with value objects

### WMS Service
- **Event-Driven Inventory**: Real-time stock updates from POS events
- **Transaction Audit**: Complete audit trail of inventory movements
- **Low Stock Alerts**: Automatic detection and alerting

### Integration Events
- `SaleCompletedEvent`: Triggers inventory deduction
- `SaleCancelledEvent`: Restores inventory for cancelled sales
- `ReturnCreatedEvent`: Processes returns and restocks items

## 🛠️ Tech Stack

- **.NET 10** - Latest LTS framework
- **PostgreSQL** - Primary database
- **Apache Kafka** - Event streaming
- **Entity Framework Core** - ORM
- **MediatR** - CQRS and mediator pattern
- **FluentValidation** - Input validation
- **Polly** - Resilience and transient fault handling
- **Serilog** - Structured logging
- **OpenTelemetry** - Distributed tracing

## 📁 Project Structure

```
PosWmsIntegration/
├── src/
│   ├── BuildingBlocks/
│   │   ├── Common.Events/          # Integration events
│   │   ├── Common.Kafka/           # Kafka producer/consumer
│   │   ├── Common.PostgreSQL/      # EF Core extensions
│   │   ├── Common.Observability/   # Metrics, logging, tracing
│   │   └── Common.Resilience/      # Polly policies
│   └── Services/
│       ├── POS/
│       │   ├── POS.Domain/         # Domain entities, value objects
│       │   ├── POS.Application/    # Commands, queries, handlers
│       │   ├── POS.Infrastructure/ # Data access, repositories
│       │   └── POS.API/            # REST API
│       └── WMS/
│           ├── WMS.Domain/         # Inventory entities
│           ├── WMS.Infrastructure/ # Data access
│           └── WMS.Consumer/       # Event consumer worker
├── infrastructure/
│   └── postgres/
│       └── init.sql                # Database initialization
├── docker-compose.yml              # Local development
└── PosWmsIntegration.sln           # Solution file
```

## 🚀 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/)
- [Docker Compose](https://docs.docker.com/compose/)

### Running Locally

1. **Clone the repository**
   ```bash
   git clone https://github.com/your-org/pos-wms-integration.git
   cd pos-wms-integration/PosWmsIntegration
   ```

2. **Start infrastructure services**
   ```bash
   docker-compose up -d postgres kafka kafka-ui seq
   ```

3. **Apply database migrations** (after services are up)
   ```bash
   dotnet ef database update --project src/Services/POS/POS.Infrastructure --startup-project src/Services/POS/POS.API
   dotnet ef database update --project src/Services/WMS/WMS.Infrastructure --startup-project src/Services/WMS/WMS.Consumer
   ```

4. **Run the services**
   ```bash
   # Terminal 1 - POS API
   dotnet run --project src/Services/POS/POS.API

   # Terminal 2 - WMS Consumer
   dotnet run --project src/Services/WMS/WMS.Consumer
   ```

5. **Access the services**
   - POS API: http://localhost:5001 (Swagger UI)
   - Kafka UI: http://localhost:8090
   - Seq (Logs): http://localhost:5341

### Running with Docker

```bash
docker-compose up -d
```

## 📝 API Examples

### Create a Sale

```bash
curl -X POST http://localhost:5001/api/v1/sales \
  -H "Content-Type: application/json" \
  -d '{
    "storeId": "STORE001",
    "terminalId": "TERM001",
    "cashierId": "CASH001",
    "items": [
      {
        "sku": "SKU-12345",
        "productName": "Sample Product",
        "quantity": 2,
        "unitPrice": 15000,
        "taxRate": 0.11
      }
    ]
  }'
```

### Complete a Sale

```bash
curl -X POST http://localhost:5001/api/v1/sales/{id}/complete \
  -H "Content-Type: application/json" \
  -d '{
    "payments": [
      {
        "paymentMethod": "cash",
        "amount": 33300
      }
    ]
  }'
```

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## 📊 Observability

- **Logs**: Structured logging with Serilog, shipped to Seq
- **Metrics**: OpenTelemetry metrics exported to Prometheus
- **Tracing**: Distributed tracing with correlation IDs

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
