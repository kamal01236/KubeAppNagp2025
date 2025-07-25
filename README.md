# Multi-Tier .NET 8 Web API on Kubernetes

This repository contains a sample .NET 8 Web API (Service Tier) and SQL Server (Database Tier) deployed using Kubernetes.

## 🏗 Project Structure

- `src/ServiceApi`: .NET 8 Web API connecting to SQL Server
- `manifests/`: Kubernetes deployment files (Deployments, Services, Ingress, ConfigMap, Secrets, PVC)
- `Dockerfile`: Build and publish .NET Web API Docker image (located in `src/ServiceApi`)

## 🚀 Setup Instructions

### 1. Build and Push Docker Image
```bash
# Build ServiceApi from root, using its Dockerfile
cd src/ServiceApi
#docker build -t kamal01236/service-api:latest .
docker build --no-cache -t kamal01236/service-api:latest . # forces a clean rebuild from scratch (no cached layers)
#docker build -t kamal01236/service-api .
docker push kamal01236/service-api
```

### 2. Deploy to Kubernetes
```bash
cd ../../manifests

kubectl apply -f sqlserver-configmap.yaml
kubectl apply -f sqlserver-secret.yaml
kubectl apply -f sqlserver-pvc.yaml
kubectl apply -f sqlserver-deployment.yaml
kubectl apply -f sqlserver-service.yaml

#kubectl apply -f service-api-appsettings-configmap.yaml
kubectl apply -f service-api-deployment.yaml
kubectl apply -f service-api-service.yaml
kubectl apply -f service-api-ingress.yaml

#ignore below test instructions
kubectl get pods
kubectl get pods --all-namespaces
kubectl exec -it service-api-66864898d5-47t8s -- env | grep ConnectionStrings__DefaultConnection
kubectl exec -it sqlserver-8688567587-mrpm9  -- /bin/sh
kubectl logs service-api-66864898d5-795ph
kubectl rollout restart deployment service-api
kubectl rollout status deployment service-api

```

> Ensure an Ingress controller is running and add `127.0.0.1 service-api.local` to your `/etc/hosts` file.

### 3. Migrate Database (Run Once) Apply the Job and Run:
```bash
kubectl apply -f ef-migrate-job.yaml
kubectl logs job/ef-migrator #You can watch logs:
kubectl delete job ef-migrator #After completion, you may clean up:
```

### 4. Test API
```bash
curl http://service-api.local/api/users/getall
```

## ✅ Features
- .NET 8 Web API with Entity Framework Core
- Configurable connection string via ConfigMap (for dev/test; use Secret for production)
- Secrets for database credentials (recommended for production)
- PVC for persistent SQL Server storage
- Rolling updates enabled for API deployment (4 replicas)
- Single replica for SQL Server with persistent storage
- Ingress exposes service externally

## 📁 Key Manifests

- `service-api-deployment.yaml`: Deploys 4 replicas of the API with rolling updates.
- `service-api-configmap.yaml`: Provides non-sensitive configuration (connection string).
- `sqlserver-deployment.yaml`: Deploys SQL Server with 1 replica.
- `sqlserver-pvc.yaml`: Persistent storage for SQL Server.
- `sqlserver-secret.yaml`: (Optional) Store sensitive DB credentials.
- `service-api-ingress.yaml`: Ingress resource for external access.

---

**Note:**  
- For production, move sensitive data (like DB passwords) from ConfigMap to Secret and reference it in your deployment.
- The manifests directory contains all Kubernetes resources needed for

