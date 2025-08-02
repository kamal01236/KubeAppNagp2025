# Multi-Tier .NET 8 Web API on Kubernetes

This repository contains a sample .NET 8 Web API (Service Tier) and SQL Server (Database Tier) deployed using Kubernetes.

## Project Structure

- `src/ServiceApi`: .NET 8 Web API that connects to SQL Server
- `manifests/`: All Kubernetes manifests (Deployments, Services, Ingress, ConfigMap, Secrets, PVC)
- `Dockerfile`: Located in src/ServiceApi, builds and publishes the API image

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
# Delete Ingress
kubectl delete -f service-api-ingress.yaml

# Delete API
kubectl delete -f service-api-deployment.yaml
kubectl delete -f service-api-service.yaml

# Delete SQL Server
kubectl delete -f sqlserver-deployment.yaml
kubectl delete -f sqlserver-service.yaml
kubectl delete -f sqlserver-pvc.yaml

# Delete Configs
kubectl delete -f sqlserver-configmap.yaml
kubectl delete -f sqlserver-secret.yaml

# Optional: Delete EF migration job if applied
kubectl delete -f ef-migrate-job.yaml --ignore-not-found


# 1. Apply config and secret
kubectl apply -f sqlserver-configmap.yaml
kubectl apply -f sqlserver-secret.yaml

# 2. Deploy SQL Server
kubectl apply -f sqlserver-pvc.yaml
kubectl apply -f sqlserver-deployment.yaml
kubectl apply -f sqlserver-service.yaml

# Wait for SQL pod to be ready
kubectl get pods -l app=sqlserver -w

# 3. Deploy API
kubectl apply -f service-api-deployment.yaml
kubectl apply -f service-api-service.yaml
kubectl apply -f service-api-ingress.yaml

# 4. Optional: Run EF migration job once
kubectl apply -f ef-migrate-job.yaml
kubectl logs job/ef-migrator #You can watch logs:
kubectl delete job ef-migrator #After completion, you may clean up:

```

> Ensure an Ingress controller is running and add `127.0.0.1 service-api.local` to your `/etc/hosts` file.
> OR If using minikube then ensure `<minikube ip> service-api.local` minikube ip add in wsl instance `/etc/hosts` file 
> If running on minikube ensure enabled ingress addons `minikube addons enable ingress`

### 3. Test the API

- curl http://service-api.local/api/users/getall  
  (From host machine after adding service-api.local to /etc/hosts)

---

### 🐛 Debug with Busybox Pod (Temporary Pod)

- `kubectl run busybox --rm -it --image=busybox --restart=Never -- /bin/sh`  
  (Launch a temporary busybox pod)

Inside the busybox shell:
- `wget -qO- http://service-api/api/users/getall`  
  (Access the service via internal DNS)

- `wget --header="Host: service-api.local" http://<INGRESS-CONTROLLER-CLUSTER-IP>/api/users/getall`  
  (Simulate Ingress with custom Host header)
- wget --header="Host: service-api.local" http://104.197.56.201/api/users/getall
- `kubectl get svc -n ingress-nginx`  
  (Get the ingress controller’s cluster IP)

---

### 🧪 Supporting Debug Commands

- `kubectl get svc service-api` 
  (Get service IP and port mappings)

- `kubectl get pods`  
  (List all pods in current namespace)

- `kubectl get pods --all-namespaces`  
  (List all pods in all namespaces)

- `kubectl exec -it <api-pod-name> -- env | grep ConnectionStrings__DefaultConnection`  
  (Verify environment variable injection)

- `kubectl exec -it <sql-pod-name> -- /bin/sh`  
  (Access the SQL Server container shell)

- `kubectl logs <api-pod-name>`  
  (View logs from API pod)

- `kubectl rollout restart deployment service-api`  
  (Manually restart the deployment)

- `kubectl rollout status deployment service-api`  
  (Watch rollout progress)
- `kubectl get svc service-api`
- `kubectl get deploy service-api`
- `kubectl describe ingress service-api-ingress`
- gcloud compute firewall-rules create allow-nodeport --allow tcp:30080

✅ Install Ingress Controller (if not already installed)
If you're using NGINX Ingress (which is not installed by default in GKE):

bash
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.10.0/deploy/static/provider/cloud/deploy.yaml

This deploys the NGINX Ingress Controller on GCP with proper Service of type LoadBalancer.

Verify it's running:

bash
kubectl get pods -n ingress-nginx
kubectl get svc -n ingress-nginx


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
- `service-api-ingress.yaml`: Configures an Ingress rule to expose the API externally via `service-api.local`, using NGINX ingress controller.
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

