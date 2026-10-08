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

### 1. Start and select a cluster

Start a Kubernetes cluster first. In Docker Desktop, enable Kubernetes in **Settings → Kubernetes** and wait for the cluster status to show that it is running. Check that `kubectl` is connected to the intended cluster:

```powershell
kubectl config current-context
kubectl get nodes
```

For Docker Desktop, the context is normally `docker-desktop`. A node should report `Ready` before you continue.

### 2. Make the image available

The Deployment uses the image tag `kubernetes-example:1.0.1`, with `imagePullPolicy: IfNotPresent`. Build the image before applying the manifests:

```powershell
docker build -t kubernetes-example:1.0.1 .
```

Docker Desktop Kubernetes can use images built into Docker Desktop's local image store. For another local cluster, load the image into that cluster using its image-loading workflow. For a remote cluster, push the image to a registry and update the `image` value in `k8s/deployment.yaml` to the registry path and tag.

### 3. Deploy and check status

Apply the manifests:

```powershell
kubectl apply -f k8s/
kubectl rollout status deployment/kubernetes-example
kubectl get pods,service -l app=kubernetes-example
```

The Deployment starts **2 replicas**. Each pod listens on container port `8080`. Kubernetes uses `/health/ready` to decide whether a pod can receive traffic and `/health/live` to detect a process that needs restarting. Resource requests are `100m` CPU and `128Mi` memory; limits are `500m` CPU and `256Mi` memory.

### 4. Open the dashboard locally

The Service is `ClusterIP`, so it is reachable inside the cluster but is not exposed directly on your computer or the public internet. Use `kubectl port-forward` to create a temporary local connection. Run this in its own terminal and leave that terminal open:

```powershell
kubectl port-forward service/kubernetes-example 8082:80
```

Open `http://localhost:8082/` for the dashboard. The local port `8082` forwards to Service port `80`, which routes to container port `8080`. The Docker Compose example publishes local port `8080`, so using `8082` avoids a port conflict when both examples are running.

The port-forward only lasts while its command is running. Stop it with **Ctrl+C**. To check the endpoints directly, open `/health/live`, `/health/ready`, or `/weatherforecast` on the same host and port.

### Scale and stop

Change the number of running pods without deleting the Deployment:

```powershell
kubectl scale deployment/kubernetes-example --replicas=1
kubectl get pods -l app=kubernetes-example
```

Scale to zero to stop the app while keeping the Kubernetes resources:

```powershell
kubectl scale deployment/kubernetes-example --replicas=0
```

Delete the Deployment and Service entirely when finished:

```powershell
kubectl delete -f k8s/
```

### Troubleshooting

- **No current context or connection refused:** start the cluster, then check `kubectl config current-context` and `kubectl get nodes`.
- **Pods show `ImagePullBackOff` or `ErrImagePull`:** make sure the image tag in `k8s/deployment.yaml` exists in an image store or registry accessible to the cluster.
- **Port-forward reports that the Service has no endpoints:** check `kubectl get pods -l app=kubernetes-example` and wait for pods to become `Ready`.
- **Local port is already in use:** choose another local port, for example `kubectl port-forward service/kubernetes-example 8084:80`, then open `http://localhost:8084/`.
- **Docker Compose and Kubernetes at the same time:** Compose serves the app on local port `8080`; use a different local port such as `8082` for Kubernetes forwarding.