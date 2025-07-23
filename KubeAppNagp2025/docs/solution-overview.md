# Solution Documentation

## 📌 Requirements Understanding

- Multi-tier: One service (API) + one SQL DB
- Containerized deployment with Kubernetes
- Secure secrets (DB password via Secret)
- Configurable DB connection (via ConfigMap)
- Persistent data (SQL PVC)
- API exposed externally via Ingress
- Service API tier: 4 replicas, rolling updates enabled
- Database tier: 1 replica, persistent storage

## 🎯 Assumptions

- Kubernetes cluster has Ingress controller (like nginx) setup
- Docker Hub credentials are available to push image
- DNS or `/etc/hosts` mapping is available for `service-api.local`
- Using .NET 8 for the Web API

## 🧩 Solution Overview

- .NET 8 Web API uses Entity Framework Core to connect to SQL Server.
- API reads DB connection string from `ConfigMap`, password from `Secret` (recommended for production).
- SQL Server persists data via PVC.
- API is exposed externally using Ingress controller.
- Service API is deployed as 4 replicas with rolling update strategy.
- SQL Server is deployed as a single replica with persistent storage.

## ⚙️ Justification

- **Entity Framework Core** provides a lightweight ORM for DB interaction.
- **SQL Server 2019 container** provides industry-standard RDBMS support.
- **Kubernetes ConfigMap + Secret** provides secure and flexible configuration.
- **PVC** ensures durability across pod restarts.
- **Deployment with rolling updates** ensures zero downtime for API tier.
- **Single replica for DB** is standard for stateful workloads; PVC ensures data is not lost on pod restarts.

## 📁 Key Kubernetes Manifests

- `nagp-service-api-deployment.yaml`: Deploys 4 replicas of the API with rolling updates.
- `nagp-service-api-configmap.yaml`: Provides non-sensitive configuration (connection string).
- `nagp-service-api-secret.yaml`: (Optional) Store sensitive DB credentials.
- `nagp-service-api-service.yaml`: ClusterIP service for API.
- `nagp-service-api-ingress.yaml`: Ingress resource for external access.
- `sqlserver-deployment.yaml`: Deploys SQL Server with 1 replica.
- `sqlserver-pvc.yaml`: Persistent storage for SQL Server.
- `sqlserver-secret.yaml`: Store SQL Server credentials.
- `sqlserver-service.yaml`: ClusterIP service for SQL Server.
