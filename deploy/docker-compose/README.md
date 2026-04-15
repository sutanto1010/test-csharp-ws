# Docker Compose Deployment

This deployment runs the `RealtimeDemo.Api` container with Docker Compose.

## Files

- `docker-compose.yml`: builds the API image from the repository root `Dockerfile`
- `.env.example`: example environment values for the compose project

## Start

From this directory:

```bash
cp .env.example .env
docker compose up --build -d
```

The API will be available at:

- `http://localhost:8080/swagger`
- `http://localhost:8080/health`
- `ws://localhost:8080/ws/notes`

## Stop

```bash
docker compose down
```

## Notes

- Change `HOST_PORT` in `.env` if port `8080` is already in use on the host.
- The application currently uses in-memory storage, so notes are lost when the container restarts.
