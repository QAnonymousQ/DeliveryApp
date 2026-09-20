# DeliveryApp
Delivery orders management: ASP.NET 9 + EF Core (SQLite) backend, Vite + React 19 frontend.
xUnit integration tests in `backend.Tests`.

## Prerequisites

- **To run natively:** [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) and [Node.js 22+](https://nodejs.org/)
- **To run via Docker:** [Docker Desktop](https://www.docker.com/products/docker-desktop/)

## 1) Run without Docker

Two terminals:

```powershell
# 1) Backend API
cd backend
dotnet run            # -> http://localhost:5139

# 2) Frontend Vite dev server
cd frontend
npm install
npm run dev           # -> http://localhost:5173
```

## 2) Run with Docker

```powershell
cd DeliveryApp
docker compose up --build
```

- App: http://localhost:8080
- Backend API stays internal (http://backend:8080 on the compose network)
- SQLite data persists in the `deliveryapp_delivery-sqlite` volume

Other commands:

```powershell
docker compose logs -f backend   # view backend logs
docker compose down              # stop, keep data
docker compose down -v           # stop and DELETE the database volume
docker compose up --build        # rebuild after source changes
```

## Data & persistence

- Native run stores data in `backend/delivery.db` (local file).
- Docker stores it in the `deliveryapp_delivery-sqlite` volume.
- The two databases are independent — data does not carry over automatically.
- To move a local DB into Docker once:

```powershell
docker compose stop backend
docker run --rm -u root -v "$pwd/backend:/src:ro" -v deliveryapp_delivery-sqlite:/data alpine `
  sh -c "rm -f /data/delivery.db* && cp /src/delivery.db /data/delivery.db && chown 1654:1654 /data/delivery.db"
docker compose start backend
```

## Tests

```powershell
dotnet test DeliveryApp.slnx
```

## Project layout

```
backend/          ASP.NET 9 API (controllers, EF Core + SQLite, migrations)
backend.Tests/    xUnit unit + integration tests
frontend/         Vite + React 19 (SPA, nginx reverse proxy in Docker)
.github/workflows/CI: build + tests (backend), lint + build (frontend)
