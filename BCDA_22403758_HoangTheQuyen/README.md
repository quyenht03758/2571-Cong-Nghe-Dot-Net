# K8s Manager - BCDA Project

## Hướng dẫn chạy từng bước

### Bước 1: Kiểm tra và khởi động Kubernetes

```bash
# Kiểm tra kubectl đã cài chưa
kubectl version --client

# Kiểm tra K8s cluster đang chạy chưa
kubectl cluster-info

# Nếu chưa chạy: Enable Kubernetes trong Docker Desktop
# Docker Desktop → Settings → Kubernetes → Enable Kubernetes → Apply & Restart

# Kiểm tra nodes
kubectl get nodes
```

### Bước 2: Setup Database

```bash
# 1. Tạo database
sqlcmd -S "(localdb)\MSSQLLocalDB" -i Api\Database\create_database.sql

# 2. Tạo tables
sqlcmd -S "(localdb)\MSSQLLocalDB" -d K8sManager -i Api\Database\create_tables.sql

# 3. Seed dữ liệu mẫu
sqlcmd -S "(localdb)\MSSQLLocalDB" -d K8sManager -i Api\Database\seed_data.sql
```

### Bước 3: Chạy Backend (API)

```bash
cd Api
dotnet restore
dotnet run
```

API sẽ chạy tại: `http://localhost:5000`

#### Test API:

```bash
# Health check
curl http://localhost:5000/api/health

# Login
curl -X POST http://localhost:5000/api/auth/login -H "Content-Type: application/json" -d "{\"username\":\"admin\",\"password\":\"Admin123!\"}"

# Get current user (cần token từ login)
curl http://localhost:5000/api/auth/me -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

### Bước 4: Chạy Frontend (Blazor WebAssembly)

```bash
cd BlazorWeb
dotnet restore
dotnet run
```

Frontend sẽ chạy tại: `http://localhost:5106`

---

