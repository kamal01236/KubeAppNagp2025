
# 📘 NAGP Assignment - Multi-tier Kubernetes Deployment

## ✅ Requirement Understanding

This project implements a multi-tier architecture on Kubernetes, consisting of:

- Multi-tier: One service (A **.NET 8 API microservice** that exposes a RESTful endpoint) + one DB (A **SQL Server 2019 database** that stores persistent records)
- Containerized deployment with Kubernetes
- Secure secrets (DB password via Secret)
- Configurable DB connection (via ConfigMap)
- Persistent data (SQL PVC)
- API exposed externally via Ingress
- Service API tier: 4 replicas, rolling updates enabled
- Database tier: 1 replica, persistent storage

### Key Features

- API connects to DB via EF Core using ConfigMap and Secret
- API is exposed externally via Ingress
- Database uses a PersistentVolumeClaim (PVC)
- Application supports pod-level failure recovery and rolling updates

---

## 🎯 Assumptions

- A **Google Kubernetes Engine (GKE)** cluster is already provisioned with the following:
  - Minimum 2–3 nodes with enough resources to run multiple pods and persistent volumes.
  - Kubernetes version supports Deployments, Ingress, Secrets, ConfigMaps, PVC, and Jobs (e.g., v1.25+).

- An **NGINX Ingress Controller** is installed and exposed via a **LoadBalancer** service:
  - The external IP of the LoadBalancer is used for accessing the API publicly.

- A valid **default StorageClass** is available in the GKE cluster (typically `standard`) for dynamic PVC provisioning.

- The developer environment includes the following:
  - `gcloud` CLI is installed and authenticated to access GKE.
  - `kubectl` is configured and has the correct context set for the GKE cluster.
  - Docker is authenticated to push and pull from **Docker Hub**.

- API Docker image is built and pushed to Docker Hub and accessible from the cluster.

- GCP firewall rules allow HTTP (port 80) access to the Ingress external IP.

---

## 🔧 Solution Overview

### Architecture

- **Service API Tier**
  - Developed using .NET 8
  - Connects to SQL Server using Entity Framework Core
  - Read configuration via `ConfigMap` (host, DB name)
  - Read credentials via `Secret` (username, password)
  - Deployed as 4 replicas with rolling updates enabled
  - Exposed via Ingress

- **Database Tier**
  - SQL Server 2019 container
  - 1 replica only
  - Mounted with a Persistent Volume
  - Credentials stored as Kubernetes Secret
  - Initialized using a Kubernetes `Job` to apply EF migrations and seed data
---

## 📂 Repository Contents

├── manifests/
│ ├── ef-migrate-job.yaml
│ ├── service-api-deployment.yaml
│ ├── service-api-service.yaml
│ ├── service-api-ingress.yaml
│ ├── sqlserver-deployment.yaml
│ ├── sqlserver-service.yaml
│ ├── sqlserver-configmap.yaml
│ ├── sqlserver-secret.yaml
│ └── sqlserver-pvc.yaml
├── src/ServiceApi
│   ├── Dockerfile
├── README.md
└── docs/ServiceApi
│   ├── document.md <-- (this file)

---

## 💬 Justification of Choices

| Component                | Justification |
|--------------------------|---------------|
| **.NET 8**               | Lightweight, modern, and highly performant for APIs |
| **SQL Server**           | Reliable RDBMS with wide ecosystem support |
| **Entity Framework Core**| Enables clean DB integration, migration, and seeding |
| **ConfigMap**            | Keeps connection details outside code |
| **Secret**               | Ensures DB credentials are not exposed |
| **PVC**                  | Retains data across pod restarts |
| **Ingress**              | Provides clean external access to service |
| **Rolling Updates**      | Ensures no downtime during API updates |
| **4 API Pods**           | Handles load and ensures high availability |
| **1 DB Pod + PVC**       | Standard pattern for stateful apps with persistent data |

---

## 📁 Key Kubernetes Manifests

- `ef-migrate-job.yaml`: EF Core migration job to initialize or update the SQL Server schema.
- `service-api-deployment.yaml`: Deploys 4 replicas of the API with rolling updates.
- `service-api-service.yaml`: ClusterIP service for API.
- `service-api-ingress.yaml`: Ingress resource for external access.
- `sqlserver-configmap.yaml`: Stores non-sensitive SQL Server settings like host, database name, and username.
- `sqlserver-deployment.yaml`: Deploys SQL Server with 1 replica.
- `sqlserver-pvc.yaml`: Persistent storage for SQL Server.
- `sqlserver-secret.yaml`: Store SQL Server credentials.
- `sqlserver-service.yaml`: ClusterIP service for SQL Server.

---

## 🔗 Links

- **Code Repository**: [GitHub Repository Link Here]
- **Docker Hub Image**: [Docker Hub URL Here]
- **Service URL**: http://service-api.local/api/records

---

## 📹 Video Recording Checklist

✅ Show:
- All resources running (`kubectl get all,cm,secret`)
- API call showing data retrieval from DB
- Deleting API pod and verifying it recovers
- Deleting DB pod and verifying data persists

---

## 🧪 Test Commands

```bash
# Access the API
curl http://service-api.local/api/users/getall

# Delete API pod
kubectl delete pod <api-pod-name>

# Delete DB pod
kubectl delete pod <sql-pod-name>
```

---

## 🧼 Cleanup
-To avoid extra cost or usage:

```bash
kubectl delete -f manifests/
```

---

## 📌 Note
This project does not use any client/company code. Built solely for academic purposes under NAGP assignment.
