# Enterprise AI Operations Platform

## Problem statement
Operations teams in multi-tenant organizations track work across scattered tools, with little
visibility into what is happening, who owns it, or why something failed. Adding AI to this
kind of system is risky without strong controls: answers must be grounded in the tenant's own
documents, AI actions must be authorized and auditable, and cost must be measurable.

This platform provides one place to create, track and automate work items, with strict tenant
isolation. It is built as a production-style system, not a demo: versioned APIs, SQL persistence,
automated tests, observability, and a deployable Azure path. AI capabilities (cited document
Q&A, a bounded approval-based agent, and narrowly scoped MCP tools) are added on top of that
foundation, each with evaluation, security controls and cost tracking.

## Intended users
- **Operations staff** at tenant organizations, who create and track work items and ask questions
  about their organization's documents.
- **Tenant administrators**, who manage their organization's data and approve sensitive
  AI-driven actions.
- **Platform engineers**, who deploy, monitor and operate the system, and review its audit
  trail, cost and reliability metrics.

## Technology choices
- .NET 10, ASP.NET Core Web API
- SQL Server 2022 (Docker) with EF Core
- xUnit and WebApplicationFactory for integration tests
- Azure (planned): Bicep, Managed Identity, Key Vault, Application Insights

## Prerequisites
- .NET 10 SDK
- Docker Desktop
- EF Core CLI tool: `dotnet tool install --global dotnet-ef`

## Run locally

### 1. Start SQL Server
```powershell
$env:MSSQL_SA_PASSWORD = "<choose-a-strong-password>"
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=$env:MSSQL_SA_PASSWORD" `
  -p 1433:1433 --name sql-platform `
  -v sqlplatform-data:/var/opt/mssql `
  -d mcr.microsoft.com/mssql/server:2022-latest
```

### 2. Configure the connection string (never commit it)
```powershell
cd src/Platform.Api
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:Default" "Server=localhost,1433;Database=PlatformDb;User Id=sa;Password=<your-password>;TrustServerCertificate=True"
```

### 3. Create the database
```powershell
dotnet ef database update
```

### 4. Run the API
```powershell
dotnet run
```
Open the Swagger UI at the URL shown in the console. Health check: `GET /health`.

## Run the tests
```powershell
dotnet test
```
Tests use a separate `PlatformDb_Test` database, recreated on each run.

## API
| Method | Path | Description |
|---|---|---|
| GET | /health | Health check |
| GET | /api/work-items | List work items |
| POST | /api/work-items | Create a work item (400 on invalid input) |

## Architecture decisions
Decision records will be kept in the `adr/` folder.
InProgress