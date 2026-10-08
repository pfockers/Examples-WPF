# Project guidance

- This is a .NET 8 ASP.NET Core minimal API sample packaged with Docker and Kubernetes.
- Keep the application stateless and dependency-free unless the project requirements change.
- The container listens on port 8080. Preserve the `/health/live` and `/health/ready` endpoints used by Kubernetes probes.
- Keep Kubernetes labels and selectors consistent across the Deployment and Service.
- Build the project with `dotnet build` and check container configuration against the Dockerfile before changing it.