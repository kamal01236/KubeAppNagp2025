# Multi-Tier .NET 8 Web API on Kubernetes

This repository contains a sample .NET 8 Web API (Service Tier) and SQL Server (Database Tier) deployed using Kubernetes.

## 🔗 Links
- 🐙 GitHub Repository: [https://github.com/kamal01236/KubeAppNagp2025]
- 📦 Docker Images: Service API: `docker.io/kamal01236/service-api`
- 🌐 API URL: http://35.239.60.4/api/users/getall

## Project Structure

- `src/ServiceApi`: .NET 8 Web API that connects to SQL Server
- `manifests/`: All Kubernetes manifests (Deployments, Services, Ingress, ConfigMap, Secrets, PVC)
- `Dockerfile`: Located in src/ServiceApi, builds and publishes the API image

## 🚀 Endpoints
- `GET /api/users/getall` – Fetch all records.
- `POST /api/users/add  ` – Add a new record.

## Setup Instructions

### 1. Build and Push Docker Image
```bash
# Build ServiceApi from root, using its Dockerfile
cd src/ServiceApi
docker build -t kamal01236/service-api:latest .
docker push kamal01236/service-api:latest
```

### 2. Deploy to Kubernetes
```bash
cd ../../manifests
# 1. Apply config and secret
kubectl apply -f sqlserver-configmap.yaml
kubectl apply -f sqlserver-secret.yaml

# 2. Deploy SQL Server
kubectl apply -f sqlserver-pvc.yaml
kubectl apply -f sqlserver-deployment.yaml
kubectl apply -f sqlserver-service.yaml

# Wait for SQL pod to be ready
kubectl get pods -l app=sqlserver -w

# 3. Run migration job (create table + seed data)
kubectl apply -f ef-migrate-job.yaml
#kubectl logs job/ef-migrator #You can watch logs:
#kubectl delete job ef-migrator #After completion, you may clean up:

# 4. Deploy API
kubectl apply -f service-api-deployment.yaml
kubectl apply -f service-api-service.yaml

# 5. Setup Ingress
kubectl apply -f service-api-ingress.yaml

```

### 3. Test the API

- curl http://35.239.60.4/api/users/getall  

---

## ✅ Features
- .NET 8 Web API with Entity Framework Core
- Configurable connection string via ConfigMap (for dev/test; use Secret for production)
- Secrets for database credentials (recommended for production)
- PVC for persistent SQL Server storage
- Rolling updates enabled for API deployment (4 replicas)
- Single replica for SQL Server with persistent storage
- Ingress exposes service externally

## 📁 Key Manifests

- `ef-migrate-job.yaml`: One-time EF Core migration job to initialize or update the SQL Server schema.
- `service-api-deployment.yaml`: Deploys 4 replicas of the .NET 8 Web API with environment-based configuration overrides and rolling update strategy.
- `service-api-ingress.yaml`: Configures an Ingress rule to expose the API externally using NGINX ingress controller.
- `service-api-service.yaml`: NodePort service that exposes the service-api pods externally on port 30080 and internally on port 80 (mapped to container port 8080).
- `sqlserver-configmap.yaml`: Stores non-sensitive SQL Server settings like host, database name, and username.
- `sqlserver-deployment.yaml`: Runs a single SQL Server container configured to use a Persistent Volume Claim (PVC).
- `sqlserver-pvc.yaml`: Defines 1Gi persistent volume for SQL Server data to ensure durability across pod restarts.
- `sqlserver-secret.yaml`: Securely stores sensitive database credentials (e.g., SA password).
- `sqlserver-service.yaml`: Exposes SQL Server internally within the cluster using ClusterIP on port 1433.

---

**Note:**  
- For production, move sensitive data (like DB passwords) from ConfigMap to Secret and reference it in your deployment.
- The manifests directory contains all Kubernetes resources needed for