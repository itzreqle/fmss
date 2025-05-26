# 🚀 Docker Services Guide

This guide walks you through running your multi-service application using Docker Compose, explaining the purpose and structure of each Compose file.

---

## 📁 Directory Overview

```
fmss/
├── compose/
│   ├── docker-compose.dev.yml  # Development services
│   ├── docker-compose.db.yml   # Database services
│   └── .env                    # Environment variables
├── database/
│   ├── postgres/init.sql       # PostgreSQL init script
│   └── clickhouse/init.sql     # ClickHouse init script
├── services/
│   ├── auth/                   # .NET Core 8 auth service
│   ├── backend/                # Node.js backend
│   └── frontend/               # Next.js + ShadCN frontend
├── k8s/                        # Kubernetes deployment configs
└── docker-compose.yml → ./compose/docker-compose.dev.yml
```

---

## 🛠️ Compose Files

### 🐳 `docker-compose.dev.yml`

> Path: `compose/docker-compose.dev.yml`

**Purpose**: Spin up **all core services** required for development — frontend, backend, auth, and optionally DBs.

**Includes:**

- `frontend` (Next.js with ShadCN)
    
- `backend` (Node.js)
    
- `auth-api` (.NET Core 8.0)
    
- (Can optionally include Postgres & ClickHouse for full stack dev)
    

**How to Run:**

```bash
docker-compose -f compose/docker-compose.dev.yml up -d
```

---

### 🗄️ `docker-compose.db.yml`

> Path: `compose/docker-compose.db.yml`

**Purpose**: Spin up **only the databases**: PostgreSQL and ClickHouse.

**Includes:**

- `postgres`
    
- `clickhouse`
    
- `.NET 8.0` auth service (optional DB access testing)
    

**How to Run:**

```bash
docker-compose -f compose/docker-compose.db.yml up -d
```

**Shutdown and Clean Volumes:**

```bash
docker-compose -f compose/docker-compose.db.yml down -v
```

---

## 🔄 When to Use Which

|Use Case|`dev.yml`|`db.yml`|
|---|---|---|
|Full stack development|✅|✅ (nested)|
|DB migration/setup only|❌|✅|
|Lightweight frontend dev|✅|❌ (mock APIs)|
|Production-like full stack|✅|✅|

---

## 📝 Environment Variables

Define environment-specific variables in `compose/.env`:

```ini
POSTGRES_USER=myuser
POSTGRES_PASSWORD=mypassword
POSTGRES_DB=mydb
CLICKHOUSE_USER=clickuser
CLICKHOUSE_PASSWORD=clickpass
CLICKHOUSE_DB=analytics
```

---

## 🐳 Initializing Databases

### PostgreSQL

Init script:

```bash
database/postgres/init.sql
```

Mounted via:

```yaml
volumes:
  - ../database/postgres:/docker-entrypoint-initdb.d
```

### ClickHouse

Init script:

```bash
database/clickhouse/init.sql
```

Mounted via:

```yaml
volumes:
  - ../database/clickhouse:/docker-entrypoint-initdb.d
```

> ⚠️ ClickHouse requires HTTP (8123) or TCP (9000) port open for queries.

---

## 📦 Auth Service (.NET 8.0)

Image:

```
mcr.microsoft.com/dotnet/runtime:8.0
```

Make sure to mount your `auth` service if you add a Dockerfile:

```yaml
build:
  context: ../services/auth
  dockerfile: Dockerfile
```

---

## 🧠 Summary

- Use `docker-compose.dev.yml` for active development.
    
- Use `docker-compose.db.yml` for initializing/testing databases.
    
- Define services modularly for future Kubernetes expansion (already scaffolded in `k8s/base/`).
    
- A `docker-compose.yml` symlink ensures a default context.
    
