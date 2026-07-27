# EduApoyos — Full Stack / Proyecto Full Stack

| | |
|---|---|
| **Technical test / Prueba técnica** | .NET Full Stack — Senior / Semi-Senior |
| **Backend** | .NET 8 · EF Core · SQL Server · JWT · Clean Architecture · CQRS |
| **Frontend** | Angular 17 · Material · JWT · jsPDF |
| **Delivery / Entrega** | Git + README + Docker |

---

## 1. Description

EduApoyos manages higher-education **economic support requests** (scholarships, credits, subsidies):

- **Advisors** manage students and requests, change status (with mandatory observation and history).
- **Students** use a self-service portal: view requests, create requests, download constancy (TXT/PDF).

### Functional coverage

| ID | Feature |
|----|---------|
| RF-01 | Authentication (login/register, roles Advisor / Student) |
| RF-02 | Student management |
| RF-03 | Support requests + status flow: Pending → UnderReview → Approved \| Rejected |
| RF-04 | Student portal + constancy (text and PDF) |
| RF-05 | List filters (status, type) + pagination |

**Business rules**

- Observation is **required** when changing status.
- **Approved** and **Rejected** are final (cannot be changed again).
- Student portal uses `GET /api/students/me` (no manual Student Id).

---

## 2. Repository structure

```
EduApoyos/
├── README.md
├── docker-compose.yml                 # optional root compose
├── EduApoyosBack/
│   ├── Dockerfile
│   ├── docker-compose.yml             # SQL Server + API
│   ├── README.md
│   ├── Scripts/                       # SQL exercises
│   ├── .github/workflows/ci.yml
│   ├── src/
│   │   ├── EduApoyos.Domain/
│   │   ├── EduApoyos.Application/
│   │   ├── EduApoyos.Infrastructure/
│   │   └── EduApoyos.API/
│   └── tests/EduApoyos.Application.Tests/
└── EduApoyosFront/
    ├── Dockerfile
    ├── nginx.conf
    ├── docker-compose.yml             # SPA on nginx
    ├── .dockerignore
    ├── README.md
    └── src/app/
        ├── core/                      # models, services, guards, interceptors
        └── features/                  # login, advisor, support-requests, student
```

---

## 3. Prerequisites

- .NET 8 SDK (local backend)
- Node.js 18 or 20 (local frontend)
- Docker Desktop (containers)

---

## 4. Demo users (seed)

| Role | Email | Password |
|------|-------|----------|
| Advisor | advisor@eduapoyos.com | Advisor123* |
| Student | student@eduapoyos.com | Student123* |

---

## 5. Run with Docker

### Backend (SQL + API)

```bash
cd EduApoyosBack
docker compose up --build -d
```

| Service | URL |
|---------|-----|
| API | http://localhost:8080 |
| Swagger | http://localhost:8080/swagger |
| SQL Server | localhost:1433 (`sa` / `Your_strong_Password123`) |

### Frontend

Configure production API URL:

```typescript
// environment.production.ts
apiUrl: 'http://localhost:8080/api'
```

```bash
cd EduApoyosFront
docker compose up --build -d
```

| Service | URL |
|---------|-----|
| Frontend | http://localhost:4200 |

**Dockerfile note:** dist folder name comes from the Angular project name, e.g.:

```dockerfile
COPY --from=build /app/dist/edu-apoyos-front/browser /usr/share/nginx/html
```

Use `.dockerignore` to exclude `node_modules`, `dist`, `.angular`.

### Useful commands

```bash
docker compose logs -f
docker compose down
docker compose down -v
```

---

## 6. Local run (without Docker)

```bash
# Backend
cd EduApoyosBack
dotnet restore
dotnet run --project src/EduApoyos.API

# Frontend
cd EduApoyosFront
npm install
npx ng serve
```

Set `apiUrl` in `environment.ts` to your API base URL.

---

## 7. Main API endpoints

| Method | Route | Auth |
|--------|-------|------|
| POST | `/api/auth/login` | Public |
| POST | `/api/auth/register` | Public |
| GET/POST | `/api/students` | Advisor |
| GET | `/api/students/me` | Student |
| GET | `/api/students/{id}/support-requests` | Student (own) |
| GET/POST | `/api/support-requests` | Advisor / Student |
| GET | `/api/support-requests/{id}` | Advisor or owner |
| PATCH | `/api/support-requests/{id}/status` | Advisor |

---

## 8. Design patterns (backend)

| Pattern | Why |
|---------|-----|
| Repository | Isolates persistence; testable |
| CQRS (MediatR) | One use case per handler; thin controllers |

---

## 9. Tests and CI

```bash
cd EduApoyosBack
dotnet test
```

CI: `EduApoyosBack/.github/workflows/ci.yml` → restore → build → test → publish.

---

## 10. SQL scripts (requirement §4.2)

In `EduApoyosBack/Scripts/`:

1. Pending requests older than 5 days  
2. Counts by status and type (last month)  
3. Non-clustered index on `(Status, UpdatedAt)`  

---

## 11. Azure (documented — deploy not required)

| Service | Justification |
|---------|----------------|
| Azure App Service | Host API / SPA |
| Azure SQL Database | Managed database |
| Azure Key Vault | Secrets (JWT, connection string) |
| Azure Blob Storage | Optional PDFs/documents |

---

## 12. Troubleshooting

| Issue | Fix |
|-------|-----|
| Docker context too large | `.dockerignore` with `node_modules`, `dist`, `.angular` |
| Style budget errors on build | Increase `anyComponentStyle` in `angular.json` |
| `dist/.../browser` not found | Use actual output path (`edu-apoyos-front`) |
| CORS errors | Allow `http://localhost:4200` in API |
| mat-select not visible | Overlay z-index + light panel CSS |

---

## 13. Evaluator checklist

- [x] Clean Architecture  
- [x] JWT + roles + resource authorization  
- [x] Status flow + history + final states  
- [x] Pagination / filters  
- [x] EF Core + SQL Server + seed  
- [x] SQL scripts + index  
- [x] Swagger  
- [x] Docker (API + SQL + Front)  
- [x] Unit tests  
- [x] CI YAML  
- [x] Azure documented  
- [x] Angular SPA (portal + constancy)  

---

## 14. Related docs

- `EduApoyosBack/README.md` — backend detail  
- `EduApoyosFront/README.md` — frontend detail  

---


## 1. Descripción

EduApoyos gestiona **solicitudes de apoyo económico** (becas, créditos, subsidios) en educación superior:

- **Asesores** administran estudiantes y solicitudes, cambian el estado (con observación obligatoria e historial).
- **Estudiantes** usan un portal de autogestión: ver solicitudes, crear solicitudes, descargar constancia (TXT/PDF).

### Cobertura funcional

| ID | Funcionalidad |
|----|----------------|
| RF-01 | Autenticación (login/registro, roles Asesor / Estudiante) |
| RF-02 | Gestión de estudiantes |
| RF-03 | Solicitudes + flujo: Pendiente → En revisión → Aprobada \| Rechazada |
| RF-04 | Portal del estudiante + constancia (texto y PDF) |
| RF-05 | Filtros (estado, tipo) + paginación |

**Reglas de negocio**

- La **observación es obligatoria** al cambiar el estado.
- **Aprobado** y **Rechazado** son estados finales (no se pueden volver a cambiar).
- El portal usa `GET /api/students/me` (sin pedir el Student Id a mano).

---

## 2. Estructura del repositorio

```
EduApoyos/
├── README.md
├── docker-compose.yml
├── EduApoyosBack/          # API .NET 8
│   ├── Dockerfile
│   ├── docker-compose.yml  # SQL + API
│   ├── Scripts/
│   ├── src/ (Domain, Application, Infrastructure, API)
│   └── tests/
└── EduApoyosFront/         # SPA Angular 17
    ├── Dockerfile
    ├── nginx.conf
    ├── docker-compose.yml
    └── src/app/ (core + features)
```

---

## 3. Prerrequisitos

- .NET 8 SDK (backend local)
- Node.js 18 o 20 (frontend local)
- Docker Desktop (contenedores)

---

## 4. Usuarios demo (seed)

| Rol | Email | Contraseña |
|-----|-------|------------|
| Advisor | advisor@eduapoyos.com | Advisor123* |
| Student | student@eduapoyos.com | Student123* |

---

## 5. Ejecutar con Docker

### Backend (SQL + API)

```bash
cd EduApoyosBack
docker compose up --build -d
```

| Servicio | URL |
|----------|-----|
| API | http://localhost:8080 |
| Swagger | http://localhost:8080/swagger |
| SQL Server | localhost:1433 (`sa` / `Your_strong_Password123`) |

### Frontend

```typescript
// environment.production.ts
apiUrl: 'http://localhost:8080/api'
```

```bash
cd EduApoyosFront
docker compose up --build -d
```

| Servicio | URL |
|----------|-----|
| Frontend | http://localhost:4200 |

**Nota Dockerfile:** la carpeta `dist` depende del nombre del proyecto Angular, por ejemplo:

```dockerfile
COPY --from=build /app/dist/edu-apoyos-front/browser /usr/share/nginx/html
```

Usa `.dockerignore` con `node_modules`, `dist`, `.angular`.

### Comandos útiles

```bash
docker compose logs -f
docker compose down
docker compose down -v
```

---

## 6. Ejecución local (sin Docker)

```bash
# Backend
cd EduApoyosBack
dotnet restore
dotnet run --project src/EduApoyos.API

# Frontend
cd EduApoyosFront
npm install
npx ng serve
```

Configura `apiUrl` en `environment.ts`.

---

## 7. Endpoints principales del API

| Método | Ruta | Auth |
|--------|------|------|
| POST | `/api/auth/login` | Público |
| POST | `/api/auth/register` | Público |
| GET/POST | `/api/students` | Advisor |
| GET | `/api/students/me` | Student |
| GET | `/api/students/{id}/support-requests` | Student (propias) |
| GET/POST | `/api/support-requests` | Advisor / Student |
| GET | `/api/support-requests/{id}` | Advisor o dueño |
| PATCH | `/api/support-requests/{id}/status` | Advisor |

---

## 8. Patrones de diseño (backend)

| Patrón | Para qué |
|--------|----------|
| Repository | Aísla la persistencia; facilita tests |
| CQRS (MediatR) | Un caso de uso por handler; controladores delgados |

---

## 9. Pruebas y CI

```bash
cd EduApoyosBack
dotnet test
```

CI: `EduApoyosBack/.github/workflows/ci.yml` → restore → build → test → publish.

---

## 10. Scripts SQL (requisito §4.2)

En `EduApoyosBack/Scripts/`:

1. Solicitudes pendientes con más de 5 días  
2. Conteo por estado y tipo (último mes)  
3. Índice no agrupado en `(Status, UpdatedAt)`  

---

## 11. Azure (documentado — no se exige despliegue)

| Servicio | Justificación |
|----------|----------------|
| Azure App Service | Hospedar API / SPA |
| Azure SQL Database | Base de datos administrada |
| Azure Key Vault | Secretos (JWT, connection string) |
| Azure Blob Storage | Opcional para PDF/documentos |

---

## 12. Solución de problemas

| Problema | Solución |
|----------|----------|
| Contexto Docker muy grande | `.dockerignore` con `node_modules`, `dist`, `.angular` |
| Error de budget en build | Subir `anyComponentStyle` en `angular.json` |
| No encuentra `dist/.../browser` | Usar la ruta real (`edu-apoyos-front`) |
| Errores CORS | Permitir `http://localhost:4200` en el API |
| mat-select no se ve | z-index del overlay + CSS de panel claro |

---

## 13. Checklist del evaluador

- [x] Clean Architecture  
- [x] JWT + roles + autorización por recurso  
- [x] Flujo de estados + historial + estados finales  
- [x] Paginación / filtros  
- [x] EF Core + SQL Server + seed  
- [x] Scripts SQL + índice  
- [x] Swagger  
- [x] Docker (API + SQL + Front)  
- [x] Pruebas unitarias  
- [x] YAML de CI  
- [x] Azure documentado  
- [x] SPA Angular (portal + constancia)  

---

## 14. Documentación relacionada

- `EduApoyosBack/README.md` — detalle del backend  
- `EduApoyosFront/README.md` — detalle del frontend  

