# RealtimeDemo

Simple ASP.NET Core application that demonstrates:

- A REST API for creating and retrieving notes
- A WebSocket endpoint for real-time notifications
- Swagger UI for API exploration
- Unit tests for service and controller behavior
- Linux-friendly .NET 10 deployment

## Architecture

- `src/RealtimeDemo.Api`: ASP.NET Core Web API
- `tests/RealtimeDemo.Api.Tests`: xUnit test project
- `INoteService`: business logic abstraction for note storage
- `IRealtimeNotifier`: abstraction for broadcasting real-time events
- `WebSocketConnectionManager`: tracks active WebSocket clients and handles broadcasts

This demo intentionally uses a small, production-friendly structure:

- Dependency injection for application services
- Controller-based REST endpoints
- `ProblemDetails` and centralized exception handling
- Health check endpoint at `/health`
- Swagger UI at `/swagger`
- WebSockets hosted directly by ASP.NET Core

## Endpoints

### REST API

- `GET /api/notes`: returns all notes, newest first
- `GET /api/notes/{id}`: returns a single note
- `POST /api/notes`: creates a new note
- `GET /health`: health check endpoint

### WebSocket

- `GET /ws/notes`: upgrades to a WebSocket connection

When a note is created through `POST /api/notes`, every connected WebSocket client receives a `noteCreated` event.

On connect, the WebSocket sends a `connected` event. If the client sends the text message `ping`, the server replies with `pong`.

## Requirements

- .NET 10 SDK
- Recommended SDK for this repo: `10.0.202`

Check the installed SDK:

```bash
dotnet --version
```

## Run Locally

From the repository root:

```bash
dotnet restore
dotnet build
dotnet run --project src/RealtimeDemo.Api
```

Default local URL from `launchSettings.json`:

```text
http://localhost:5080
```

Swagger UI:

- [http://localhost:5080/swagger](http://localhost:5080/swagger)

## Example Usage

Create a note:

```bash
curl -X POST http://localhost:5080/api/notes \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Deploy to Linux",
    "content": "Publish the service with dotnet publish."
  }'
```

Get all notes:

```bash
curl http://localhost:5080/api/notes
```

Connect to WebSocket using `wscat`:

```bash
npx wscat -c ws://localhost:5080/ws/notes
```

After a `POST /api/notes`, connected clients receive a payload similar to:

```json
{
  "type": "noteCreated",
  "data": {
    "id": "6b1cc2c5-1f18-4708-b3f1-74d29b1712d2",
    "title": "Deploy to Linux",
    "content": "Publish the service with dotnet publish.",
    "createdAtUtc": "2026-04-15T09:30:00+00:00"
  }
}
```

## Run Tests

```bash
dotnet test
```

## Docker Compose Deployment

Use the compose deployment under `deploy/docker-compose`:

```bash
cd deploy/docker-compose
cp .env.example .env
docker compose up --build -d
```

Default URLs:

- Swagger: [http://localhost:8080/swagger](http://localhost:8080/swagger)
- Health: [http://localhost:8080/health](http://localhost:8080/health)
- WebSocket: `ws://localhost:8080/ws/notes`

Stop the deployment:

```bash
cd deploy/docker-compose
docker compose down
```

## Linux Deployment

Framework-dependent publish for Linux:

```bash
dotnet publish src/RealtimeDemo.Api/RealtimeDemo.Api.csproj \
  -c Release \
  -r linux-x64 \
  --self-contained false \
  -o ./publish/linux-x64
```

Run the published app on Linux:

```bash
ASPNETCORE_URLS=http://0.0.0.0:8080 \
ASPNETCORE_ENVIRONMENT=Production \
dotnet ./publish/linux-x64/RealtimeDemo.Api.dll
```

Recommended production setup:

- Run behind Nginx, Apache, or a cloud load balancer
- Terminate TLS at the reverse proxy or ingress layer
- Enable process supervision with `systemd`, Docker, or Kubernetes
- Replace the in-memory note store with a persistent database for real workloads
- Restrict CORS and authentication based on your environment
- Keep the SDK aligned with `global.json` for reproducible local and CI builds

## Notes

- The current implementation uses in-memory storage to keep the sample easy to understand.
- WebSockets are suitable here for server push and lightweight realtime notifications.
- For larger production systems, consider moving note persistence to PostgreSQL or SQL Server and protecting endpoints with authentication and authorization.
