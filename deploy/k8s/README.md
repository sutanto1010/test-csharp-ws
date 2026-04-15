# Helm Deployment

This directory contains a Helm chart for deploying `RealtimeDemo.Api` to Kubernetes.

## Chart Layout

- `realtime-demo/Chart.yaml`: chart metadata
- `realtime-demo/values.yaml`: default configuration values
- `realtime-demo/templates/`: Kubernetes resource templates

## Build and Push the Image

Build the application image from the repository root and push it to a registry your cluster can access:

```bash
docker build -t ghcr.io/your-org/realtimedemo-api:latest .
docker push ghcr.io/your-org/realtimedemo-api:latest
```

If you use a local cluster such as Docker Desktop or `kind`, you can also load the image directly and keep `image.pullPolicy=IfNotPresent`.

## Install

From the repository root:

```bash
helm upgrade --install realtime-demo ./deploy/k8s/realtime-demo \
  --namespace realtime-demo \
  --create-namespace \
  --set image.repository=ghcr.io/your-org/realtimedemo-api \
  --set image.tag=latest
```

## Common Overrides

Enable ingress:

```bash
helm upgrade --install realtime-demo ./deploy/k8s/realtime-demo \
  --namespace realtime-demo \
  --create-namespace \
  --set image.repository=ghcr.io/your-org/realtimedemo-api \
  --set image.tag=latest \
  --set ingress.enabled=true \
  --set ingress.className=nginx \
  --set ingress.hosts[0].host=realtime.example.com
```

Scale the API:

```bash
helm upgrade --install realtime-demo ./deploy/k8s/realtime-demo \
  --namespace realtime-demo \
  --create-namespace \
  --set image.repository=ghcr.io/your-org/realtimedemo-api \
  --set image.tag=latest \
  --set replicaCount=3
```

## Validate Templates

```bash
helm lint ./deploy/k8s/realtime-demo
helm template realtime-demo ./deploy/k8s/realtime-demo
```

## Application Paths

- Swagger: `/swagger`
- Health: `/health`
- WebSocket: `/ws/notes`

The application keeps notes in memory, so data is lost when pods restart or are rescheduled.
