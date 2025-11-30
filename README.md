# Jobbie Backend

The **Jobbie Backend** powers the Jobbie job-posting platform using a microservice-oriented architecture.  
It handles authentication, job posting, notifications, and data persistence for the platform.

This repository contains the backend services responsible for core business logic and API endpoints consumed by the Jobbie Frontend.

---

## 📌 1. Overview

The backend provides:

- Authentication and authorization  
- CRUD operations for job postings  
- Notification dispatching  
- Clean service separation  
- Database-backed persistence  
- REST API endpoints for frontend consumption  

Designed for scalability, portability, and cloud deployment (Docker/Kubernetes).

---

## 🏗️ 2. Architecture

The Jobbie Backend consists of several microservices (adjust based on your final implementation):

- 🔐 **Auth Service** (Python/Django or Node.js)  
- 💼 **Job Posting Service** (C# ASP.NET Core)  
- 🔔 **Notification Service** (C# or Go)  
- 📋 **Listing Service** (Go or C#)  
- 🗄️ **Database Layer** (PostgreSQL recommended)  

Communication between services is REST (frontend) and internal REST/gRPC depending on configuration.

---

## ⚙️ 3. Tech Stack

| Service Type        | Technology                          |
|---------------------|--------------------------------------|
| Auth Service        | Python (Django REST) or Node.js      |
| Job Service         | C# ASP.NET Core                      |
| Notification        | C# or Go                             |
| API Gateway / Proxy | Nginx / Kubernetes Ingress           |
| Databases           | PostgreSQL / MongoDB                 |
| Containerization    | Docker                               |
| Orchestration       | Kubernetes (AKS, EKS, or Minikube)   |

---

## 📁 4. Project Structure

This is a general example layout; update according to your exact repo:

```
Jobbie_Backend/
├── auth-service/          # Authentication microservice
├── job-service/           # Job posting & job details service
├── notification-service/  # User notification service
├── listing-service/       # Listing/searching logic
├── docker/                # Docker and Docker Compose files
├── k8s/                   # Kubernetes manifests (if applicable)
├── scripts/               # Deployment/utility scripts
└── README.md
```

---

## 🚀 5. Getting Started

### ✔️ Prerequisites

- Docker installed  
- .NET SDK (if using C# services)  
- Python 3.x (if using Django services)  
- Go (if using Go-based services)  
- PostgreSQL running locally or in a container  

---

## 📥 6. Clone & Install

```bash
git clone https://github.com/ritsth/Jobbie_Backend.git
cd Jobbie_Backend
```

Install each microservice’s dependencies by navigating into its folder:

#### Example for C# service:

```bash
cd job-service
dotnet restore
```

#### Example for Django service:

```bash
cd auth-service
pip install -r requirements.txt
```

---

## 🔐 7. Environment Variables

Each service should have its own `.env` file.

### Example `.env` (Auth Service)

```env
DB_URL=postgresql://username:password@localhost:5432/jobbiedb
JWT_SECRET=your-secret-key
```

### Example `.env` (Job Service)

```env
CONNECTION_STRING=Server=localhost;Port=5432;Database=jobdb;User Id=postgres;Password=password;
```

---

## ▶️ 8. Running Services

### Option 1: Run Locally

Run each microservice individually:

**C# ASP.NET Service**
```bash
cd job-service
dotnet run
```

**Python Service**
```bash
cd auth-service
python manage.py runserver
```

### Option 2: Run with Docker Compose

If you have a `docker-compose.yml`:

```bash
docker compose up --build
```

### Option 3: Run with Kubernetes (recommended for microservices)

```bash
kubectl apply -f k8s/
```

---

## 🌐 9. API Endpoints

### Auth Service

```
POST /auth/login
POST /auth/register
GET  /auth/me
```

### Job Service

```
GET    /jobs
GET    /jobs/{id}
POST   /jobs
PUT    /jobs/{id}
DELETE /jobs/{id}
```

### Notification Service

```
POST /notify
```

---

## 🧪 10. Testing

Run tests in each microservice folder.

**For C# services:**
```bash
dotnet test
```

**For Python services:**
```bash
python manage.py test
```

---

## 🐳 11. Docker Support

### Example Dockerfile (C# Service)

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . .
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "JobService.dll"]
```

### Build & Run

```bash
docker build -t jobbie-job-service .
docker run -p 8001:80 jobbie-job-service
```

---

## 🤝 12. Contributing

```bash
git checkout -b feature/my-feature
```

Open a pull request once changes are committed.

---


