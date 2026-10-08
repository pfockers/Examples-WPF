# Kubernetes Example

A small ASP.NET Core 8 API demonstrating how to build a Docker image and deploy it to Kubernetes. It includes a sample weather endpoint plus health endpoints for Kubernetes probes.

The root URL serves a compact weather dashboard showing the forecast and API health status.

## Requirements

- .NET 8 SDK to run or build the API locally
- Docker Desktop (or another Docker-compatible engine) to build and run the image
- A running Kubernetes cluster and `kubectl` to deploy it

## Run locally

From this directory:

```powershell
dotnet run --urls http://localhost:8080
```

Open `http://localhost:8080/` for the dashboard. The API endpoints are `http://localhost:8080/health/live`, `http://localhost:8080/health/ready`, and `http://localhost:8080/weatherforecast`. In Development, the OpenAPI UI is available at `/swagger`.

## Build and run with Docker

Build the image:

```powershell
docker build -t kubernetes-example:1.0.1 .
```

Run it locally:

```powershell
docker run --rm -p 8080:8080 kubernetes-example:1.0.1
```

Then browse to `http://localhost:8080/health/ready`.

## Start everything in Docker with one command

Docker Compose builds the image if needed and starts the API and dashboard with port 8080 published:

```powershell
docker compose up --build -d
```

Open `http://localhost:8080/`. To stop the Compose app:

```powershell
docker compose down
```

Use either Docker Compose or the Kubernetes instructions below for the app; Kubernetes manages its own app containers.

## Deploy to Kubernetes

The manifests refer to `kubernetes-example:1.0.1`. Build that tag in an image store visible to your cluster. For Docker Desktop Kubernetes, this commonly means building the image locally in Docker Desktop. For a remote cluster, push the image to a registry and replace the `image` value in `k8s/deployment.yaml` with your registry path and tag.

Apply the manifests:

```powershell
kubectl apply -f k8s/
kubectl rollout status deployment/kubernetes-example
kubectl get pods,service -l app=kubernetes-example
```

The Service is internal to the cluster (`ClusterIP`). Forward it to localhost to open the dashboard from your machine:

```powershell
kubectl port-forward service/kubernetes-example 8080:80
```

Browse to `http://localhost:8080/` for the dashboard, or `http://localhost:8080/health/ready` to check readiness. Remove the sample resources when finished:

```powershell
kubectl delete -f k8s/
```