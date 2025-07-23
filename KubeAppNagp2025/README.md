# Multi-Tier .NET 8 Web API on Kubernetes

This repository contains a sample .NET 8 Web API (Service Tier) and SQL Server (Database Tier) deployed using Kubernetes.

## 🏗 Project Structure

- `src/NagpServiceApi`: .NET 8 Web API connecting to SQL Server
- `manifests/`: Kubernetes deployment files (Deployments, Services, Ingress, ConfigMap, Secrets, PVC)
- `Dockerfile`: Build and publish .NET Web API Docker image (located in `src/NagpServiceApi`)

## 🚀 Setup Instructions

### 1. Build and Push Docker Image
```bash
cd src/NagpServiceApi
docker build -t kamal01236/nagp-service-api .
docker push kamal01236/nagp-service-api
```

### 2. Deploy to Kubernetes
```bash
kubectl apply -f manifests/sqlserver-configmap.yaml
kubectl apply -f manifests/sqlserver-secret.yaml
kubectl apply -f manifests/sqlserver-pvc.yaml
kubectl apply -f manifests/sqlserver-deployment.yaml
kubectl apply -f manifests/sqlserver-service.yaml

kubectl apply -f manifests/nagp-service-api-configmap.yaml
kubectl apply -f manifests/nagp-service-api-deployment.yaml
kubectl apply -f manifests/nagp-service-api-service.yaml
kubectl apply -f manifests/nagp-service-api-ingress.yaml
```

> Ensure an Ingress controller is running and add `127.0.0.1 nagp-service-api.local` to your `/etc/hosts` file.

### 3. Migrate Database (Run Once)
First, get the name of any Service API pod:
```bash
kubectl get pods -l app=nagp-service-api
```
Then, run the migration command on any one of the listed pods:
```bash
kubectl exec -it <nagp-service-api-pod-name> -- dotnet ef database update
```
Replace `<nagp-service-api-pod-name>` with the actual pod name from the previous command.

### 4. Test API
```bash
curl http://nagp-service-api.local/api/users/getall
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

- `nagp-service-api-deployment.yaml`: Deploys 4 replicas of the API with rolling updates.
- `nagp-service-api-configmap.yaml`: Provides non-sensitive configuration (connection string).
- `sqlserver-deployment.yaml`: Deploys SQL Server with 1 replica.
- `sqlserver-pvc.yaml`: Persistent storage for SQL Server.
- `sqlserver-secret.yaml`: (Optional) Store sensitive DB credentials.
- `nagp-service-api-ingress.yaml`: Ingress resource for external access.

---

**Note:**  
- For production, move sensitive data (like DB passwords) from ConfigMap to Secret and reference it in your deployment.
- The manifests directory contains all Kubernetes resources needed for

