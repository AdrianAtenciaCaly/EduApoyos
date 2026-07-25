# EduApoyos Backend / Backend EduApoyos

REST API for managing economic support requests (scholarships, credits, subsidies) for higher education students.

API REST para la gestión de solicitudes de apoyo económico (becas, créditos, subsidios) de estudiantes de educación superior.

| | |
|---|---|
| **Technical test / Prueba técnica** | .NET Full Stack Developer — Senior / Semi-Senior |
| **Stack** | .NET 8 · EF Core 8 · SQL Server · JWT (Identity) · Clean Architecture · CQRS (MediatR) |

---

# English

## 1. Description

EduApoyos replaces spreadsheet and email-based processes with a traceable system:

- **Advisors** register students, create and review support requests, and change request status.
- **Students** consult only their own requests from a self-service portal (resource-based authorization).

### Functional coverage (API)

| ID | Feature | Endpoints |
|----|---------|-----------|
| RF-01 | Authentication | `POST /api/auth/login`, `POST /api/auth/register` |
| RF-02 | Student management | `GET /api/students`, `POST /api/students` |
| RF-03 | Support requests + status flow | `GET/POST /api/support-requests`, `PATCH /api/support-requests/{id}/status` |
| RF-04 | Student portal | `GET /api/students/{id}/support-requests` |
| RF-05 | List, filters, pagination | Query: `status`, `type`, `page`, `pageSize` |

**Status flow:** Pending → UnderReview → Approved | Rejected (audited in `StatusHistory`).

---

## 2. Solution structure (Clean Architecture)

```
EduApoyosBack/
├── src/
│   ├── EduApoyos.Domain/           # Entities, enums, repository interfaces
│   ├── EduApoyos.Application/      # CQRS use cases, DTOs, abstractions
│   ├── EduApoyos.Infrastructure/   # EF Core, Identity, JWT, repositories, seed
│   └── EduApoyos.API/              # Controllers, Program.cs, Swagger, CORS
├── tests/
│   └── EduApoyos.Application.Tests/
├── Scripts/                        # Required SQL exercises
├── Dockerfile
├── docker-compose.yml
└── README.md
```

**Dependency rule:** `API → Infrastructure → Application → Domain`  
Application never references Infrastructure (Dependency Inversion).

---

## 3. Design patterns

| Pattern | Where | Why |
|---------|--------|-----|
| **Repository** | `ISupportRequestRepository`, `IStudentRepository` | Isolates persistence; mockable unit tests; centralized queries |
| **CQRS (MediatR)** | Commands/Queries + Handlers | Thin controllers; one use case per class; easy to test |

Also: Unit of Work, domain method `SupportRequest.ChangeStatus`, JWT via `IJwtTokenService`.

---

## 4. Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server, LocalDB (Windows), or Docker Desktop
- (Optional) EF Core CLI:

```bash
dotnet tool install --global dotnet-ef
```

---

## 5. Local setup (without Docker)

```bash
cd EduApoyosBack
dotnet restore
```

### Database

**LocalDB (Windows)** — `appsettings.Development.json`:

```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EduApoyosDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

**SQL Server** — `appsettings.json`:

```json
"DefaultConnection": "Server=localhost,1433;Database=EduApoyosDb;User Id=sa;Password=Your_strong_Password123;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

### Migrations

```bash
dotnet ef migrations add InitialCreate -p src/EduApoyos.Infrastructure -s src/EduApoyos.API
dotnet ef database update -p src/EduApoyos.Infrastructure -s src/EduApoyos.API
```

On startup, `DataSeeder` also runs `MigrateAsync()` and seeds roles + demo users.

### Run API

```bash
dotnet run --project src/EduApoyos.API
```

Swagger: URL shown in the console (e.g. `https://localhost:7174/swagger`).

---

## 6. Docker

```bash
cd EduApoyosBack
docker compose up --build -d
```

| Service | URL |
|---------|-----|
| API | http://localhost:8080 |
| Swagger | http://localhost:8080/swagger |
| SQL Server | localhost:1433 (`sa` / `Your_strong_Password123`) |

```bash
docker compose logs -f api
docker compose down
docker compose down -v
```

Secrets in `docker-compose.yml` are for **local development only**.

---

## 7. Seed users

| Role | Email | Password |
|------|-------|----------|
| Advisor | advisor@eduapoyos.com | Advisor123* |
| Student | student@eduapoyos.com | Student123* |

---

## 8. Main endpoints

| Method | Route | Auth |
|--------|-------|------|
| POST | `/api/auth/login` | Public |
| POST | `/api/auth/register` | Public |
| GET | `/api/students` | Advisor |
| POST | `/api/students` | Advisor |
| GET | `/api/students/{id}/support-requests` | Student (own resource) |
| GET | `/api/support-requests` | Advisor |
| POST | `/api/support-requests` | Advisor / Student |
| GET | `/api/support-requests/{id}` | Advisor or owner |
| PATCH | `/api/support-requests/{id}/status` | Advisor |

```http
Authorization: Bearer {token}
```

Valid statuses: `Pending`, `UnderReview`, `Approved`, `Rejected`  
Valid types: `Scholarship`, `Credit`, `Subsidy`

**Student portal:** `{id}` must be the Student entity Id of the logged-in user.

---

## 9. Running unit tests

### Run all tests

```bash
cd EduApoyosBack
dotnet test
```

### Run only Application tests

```bash
dotnet test tests/EduApoyos.Application.Tests/EduApoyos.Application.Tests.csproj
```

### Verbose output

```bash
dotnet test --verbosity normal
```

### With code coverage (document requirement: ≥ 70% on Application)

```bash
dotnet test --collect:"XPlat Code Coverage"
```

Coverage results are written under:

```text
tests/EduApoyos.Application.Tests/TestResults/<guid>/coverage.cobertura.xml
```

### Optional: HTML coverage report (ReportGenerator)

```bash
dotnet tool install -g dotnet-reportgenerator-globaltool

reportgenerator \
  -reports:"**/coverage.cobertura.xml" \
  -targetdir:"coverage-report" \
  -reporttypes:Html

# Open coverage-report/index.html in a browser
```

### Filter by test name

```bash
dotnet test --filter "FullyQualifiedName~LoginCommandHandler"
dotnet test --filter "FullyQualifiedName~CreateSupportRequest"
```

### What the tests cover

- Create support request (success, student not found, invalid type)
- Update status (valid / invalid status)
- Login (valid / invalid credentials)
- Get by id (Advisor access, Student own resource, forbidden on others)
- Student portal authorization
- Domain `ChangeStatus` behavior

**Stack:** xUnit · Moq · FluentAssertions · coverlet

---

## 10. SQL scripts (requirement 4.2)

| File | Purpose |
|------|---------|
| `Scripts/01_PendingRequestsOlderThan5Days.sql` | Pending requests older than 5 days |
| `Scripts/02_CountByStatusAndTypeLastMonth.sql` | Counts by status and type (last month) |
| `Scripts/03_NonClusteredIndex.sql` | Non-clustered index `(Status, UpdatedAt)` |

EF also creates `IX_SupportRequests_Status_UpdatedAt` in `OnModelCreating`.

---

## 11. Security

- JWT with configurable expiration
- Passwords hashed with ASP.NET Core Identity
- Role-based authorization + resource checks for students
- Secrets in configuration / environment variables (not for production in source control)

---

## 12. Azure (minimum documented — requirement 8.3)

| Service | Justification |
|---------|----------------|
| **Azure App Service** | Hosts the .NET 8 Web API (and optionally the Angular SPA as static files or a second App Service). **Tier:** *Basic (B1)* for demos and low traffic (shared compute is enough for evaluation); *Standard (S1)* for production with slot swaps, auto-scale rules, and custom domains. **Basic config:** runtime stack `.NET 8`, HTTPS only, app settings for `ConnectionStrings__DefaultConnection` and `Jwt__*` (or Key Vault references), CORS origins limited to the frontend URL. |
| **Azure SQL Database** | Relational store for users, students, support requests and status history (EF Core). **Tier:** *Basic* (5 DTUs) for local-like / demo load (few concurrent users, seed + manual testing); *Standard S0–S2* when listing with filters/pagination and concurrent advisors grow. Firewall rules restrict access to App Service outbound IPs (or private endpoint). Connection string stored in Key Vault / App Settings, never in source. |
| **Azure Blob Storage** | Used **if** RF-04 document/PDF constancy upload is implemented: store generated PDF constancies or uploaded supporting files per request. Containers with private access; SAS or managed identity from App Service. *Not required if the constancy feature is deferred.* |
| **Azure Key Vault** | Production secrets: SQL connection string, JWT signing key, and any storage keys. App Service uses a **managed identity** with get permission on secrets; configuration uses Key Vault references so secrets are never committed to Git. |

Optional (recommended, not mandatory in the test doc): **Application Insights** for latency (NFR &lt; 800 ms) and failure tracking.

---


## 13. CI pipeline (requirement 8.2)

File: `.github/workflows/ci.yml` (GitHub Actions; equivalent steps work in Azure DevOps).

The pipeline is **documented and present in the repo**. It does not need to run on a private agent to meet the requirement.

| Step | Command |
|------|---------|
| 1. Restore dependencies | `dotnet restore` |
| 2. Build Release | `dotnet build --configuration Release` |
| 3. Unit tests | `dotnet test` |
| 4. Publish API artifact | `dotnet publish` → artifact `eduapoyos-api` |

Triggers: `push` / `pull_request` on `main` (and `develop`).

---


## 14. Decisions & next steps

**Decisions:** Clean Architecture, English naming, MediatR CQRS, Repository + UoW, careful status updates to avoid EF concurrency issues, seed on startup.

**With more time:** ProblemDetails middleware, more FluentValidation, integration tests (`WebApplicationFactory`), `GET /api/students/me/support-requests`, PDF constancy.

---

# Español

## 1. Descripción

EduApoyos reemplaza procesos basados en hojas de cálculo y correo por un sistema trazable:

- **Asesores (Advisor)** registran estudiantes, crean y revisan solicitudes de apoyo y cambian el estado.
- **Estudiantes (Student)** consultan solo sus propias solicitudes (autorización por recurso).

### Cobertura funcional (API)

| ID | Funcionalidad | Endpoints |
|----|---------------|-----------|
| RF-01 | Autenticación | `POST /api/auth/login`, `POST /api/auth/register` |
| RF-02 | Gestión de estudiantes | `GET /api/students`, `POST /api/students` |
| RF-03 | Solicitudes + flujo de estados | `GET/POST /api/support-requests`, `PATCH .../status` |
| RF-04 | Portal del estudiante | `GET /api/students/{id}/support-requests` |
| RF-05 | Listado, filtros y paginación | Query: `status`, `type`, `page`, `pageSize` |

**Flujo de estados:** Pending → UnderReview → Approved | Rejected (histórico en `StatusHistory`).

---

## 2. Estructura de la solución (Clean Architecture)

```
EduApoyosBack/
├── src/
│   ├── EduApoyos.Domain/           # Entidades, enums, interfaces de repositorio
│   ├── EduApoyos.Application/      # Casos de uso CQRS, DTOs, abstracciones
│   ├── EduApoyos.Infrastructure/   # EF Core, Identity, JWT, repositorios, seed
│   └── EduApoyos.API/              # Controladores, Program.cs, Swagger, CORS
├── tests/
│   └── EduApoyos.Application.Tests/
├── Scripts/                        # Scripts SQL exigidos por la prueba
├── Dockerfile
├── docker-compose.yml
└── README.md
```

**Regla de dependencias:** `API → Infrastructure → Application → Domain`  
Application **no** referencia Infrastructure (Inversión de dependencias).

---

## 3. Patrones de diseño

| Patrón | Dónde | Por qué |
|--------|--------|---------|
| **Repository** | `ISupportRequestRepository`, `IStudentRepository` | Aísla la persistencia; permite mocks en tests; centraliza consultas |
| **CQRS (MediatR)** | Commands/Queries + Handlers | Controladores delgados; un caso de uso por clase; fácil de testear |

También: Unit of Work, método de dominio `SupportRequest.ChangeStatus`, JWT mediante `IJwtTokenService`.

---

## 4. Prerrequisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server, LocalDB (Windows) o Docker Desktop
- (Opcional) CLI de EF Core:

```bash
dotnet tool install --global dotnet-ef
```

---

## 5. Instalación y ejecución local (sin Docker)

```bash
cd EduApoyosBack
dotnet restore
```

### Base de datos

**LocalDB (Windows)** — `appsettings.Development.json`:

```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EduApoyosDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

**SQL Server** — `appsettings.json`:

```json
"DefaultConnection": "Server=localhost,1433;Database=EduApoyosDb;User Id=sa;Password=Your_strong_Password123;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

### Migraciones

```bash
dotnet ef migrations add InitialCreate -p src/EduApoyos.Infrastructure -s src/EduApoyos.API
dotnet ef database update -p src/EduApoyos.Infrastructure -s src/EduApoyos.API
```

Al iniciar la API, `DataSeeder` ejecuta `MigrateAsync()` y carga roles + usuarios demo.

### Ejecutar la API

```bash
dotnet run --project src/EduApoyos.API
```

Swagger: URL que muestra la consola (ej. `https://localhost:7174/swagger`).

---

## 6. Docker

```bash
cd EduApoyosBack
docker compose up --build -d
```

| Servicio | URL |
|----------|-----|
| API | http://localhost:8080 |
| Swagger | http://localhost:8080/swagger |
| SQL Server | localhost:1433 (`sa` / `Your_strong_Password123`) |

```bash
docker compose logs -f api
docker compose down
docker compose down -v
```

Los secretos en `docker-compose.yml` son **solo para desarrollo local**.

---

## 7. Usuarios seed (demo)

| Rol | Email | Contraseña |
|-----|-------|------------|
| Advisor | advisor@eduapoyos.com | Advisor123* |
| Student | student@eduapoyos.com | Student123* |

---

## 8. Endpoints principales

| Método | Ruta | Auth |
|--------|------|------|
| POST | `/api/auth/login` | Público |
| POST | `/api/auth/register` | Público |
| GET | `/api/students` | Advisor |
| POST | `/api/students` | Advisor |
| GET | `/api/students/{id}/support-requests` | Student (solo recurso propio) |
| GET | `/api/support-requests` | Advisor |
| POST | `/api/support-requests` | Advisor / Student |
| GET | `/api/support-requests/{id}` | Advisor o dueño |
| PATCH | `/api/support-requests/{id}/status` | Advisor |

```http
Authorization: Bearer {token}
```

Estados válidos: `Pending`, `UnderReview`, `Approved`, `Rejected`  
Tipos válidos: `Scholarship`, `Credit`, `Subsidy`

**Portal estudiante:** el `{id}` debe ser el Id de la entidad Student del usuario autenticado.

---

## 9. Ejecutar las pruebas unitarias

### Todas las pruebas

```bash
cd EduApoyosBack
dotnet test
```

### Solo el proyecto de Application

```bash
dotnet test tests/EduApoyos.Application.Tests/EduApoyos.Application.Tests.csproj
```

### Salida detallada

```bash
dotnet test --verbosity normal
```

### Con cobertura de código (requisito del documento: ≥ 70% en Application)

```bash
dotnet test --collect:"XPlat Code Coverage"
```

Los resultados quedan en:

```text
tests/EduApoyos.Application.Tests/TestResults/<guid>/coverage.cobertura.xml
```

### Opcional: reporte HTML (ReportGenerator)

```bash
dotnet tool install -g dotnet-reportgenerator-globaltool

reportgenerator \
  -reports:"**/coverage.cobertura.xml" \
  -targetdir:"coverage-report" \
  -reporttypes:Html

# Abrir coverage-report/index.html en el navegador
```

### Filtrar por nombre de test

```bash
dotnet test --filter "FullyQualifiedName~LoginCommandHandler"
dotnet test --filter "FullyQualifiedName~CreateSupportRequest"
```

### Qué cubren las pruebas

- Crear solicitud (éxito, estudiante no encontrado, tipo inválido)
- Cambiar estado (válido / inválido)
- Login (credenciales válidas / inválidas)
- Obtener por id (Advisor, Student dueño, Student no autorizado)
- Autorización del portal del estudiante
- Comportamiento de dominio `ChangeStatus`

**Stack:** xUnit · Moq · FluentAssertions · coverlet

---

## 10. Scripts SQL (requisito 4.2)

| Archivo | Propósito |
|---------|-----------|
| `Scripts/01_PendingRequestsOlderThan5Days.sql` | Solicitudes pendientes con más de 5 días sin actualización |
| `Scripts/02_CountByStatusAndTypeLastMonth.sql` | Conteo por estado y tipo (último mes) |
| `Scripts/03_NonClusteredIndex.sql` | Índice no agrupado `(Status, UpdatedAt)` |

EF también crea `IX_SupportRequests_Status_UpdatedAt` en `OnModelCreating`.

---

## 11. Seguridad

- JWT con expiración configurable
- Contraseñas hasheadas con ASP.NET Core Identity
- Autorización por roles + validación por recurso para estudiantes
- Secretos en configuración / variables de entorno (no en código para producción)

---

## 12. Azure (mínimo documentado — requisito 8.3)

| Servicio | Justificación |
|----------|----------------|
| **Azure App Service** | Hospeda la Web API .NET 8 (y opcionalmente el SPA Angular como estáticos o un segundo App Service). **Tier:** *Basic (B1)* para demos y bajo tráfico; *Standard (S1)* para producción (slots, autoescalado, dominio personalizado). **Configuración básica:** runtime `.NET 8`, solo HTTPS, app settings para `ConnectionStrings__DefaultConnection` y `Jwt__*` (o referencias a Key Vault), CORS limitado a la URL del frontend. |
| **Azure SQL Database** | Base relacional para usuarios, estudiantes, solicitudes e historial de estados (EF Core). **Tier:** *Basic* (5 DTUs) para carga de demo / pocas pruebas concurrentes; *Standard S0–S2* cuando crecen listados filtrados y asesores concurrentes. Firewall hacia IPs de salida del App Service (o private endpoint). Cadena de conexión en Key Vault / App Settings, nunca en el código. |
| **Azure Blob Storage** | Se usaría **si** se implementa la carga/generación de documentos o constancias PDF de RF-04: contenedores privados; acceso con SAS o identidad administrada desde App Service. *No obligatorio si la constancia queda pendiente.* |
| **Azure Key Vault** | Secretos de producción: connection string de SQL, clave de firma JWT y claves de storage. El App Service usa **managed identity** con permiso de lectura de secretos; configuración con referencias a Key Vault para no versionar secretos en Git. |

Opcional (recomendado, no exigido en el documento): **Application Insights** para latencia (NFR &lt; 800 ms) y fallos.

---


## 13. Pipeline CI (requisito 8.2)

Archivo: `.github/workflows/ci.yml` (GitHub Actions; los mismos pasos aplican en Azure DevOps).

El pipeline está **documentado y presente en el repositorio**. No es obligatorio ejecutarlo en un agente propio para cumplir el requisito.

| Paso | Comando |
|------|---------|
| 1. Restaurar dependencias | `dotnet restore` |
| 2. Compilar Release | `dotnet build --configuration Release` |
| 3. Pruebas unitarias | `dotnet test` |
| 4. Publicar artefacto de la API | `dotnet publish` → artefacto `eduapoyos-api` |

Disparadores: `push` / `pull_request` sobre `main` (y `develop`).

---


---

## 14. Decisiones y siguientes pasos

**Decisiones:** Clean Architecture, nombres en inglés, CQRS con MediatR, Repository + UoW, actualización de estados evitando concurrencia de EF, seed al arrancar.

**Con más tiempo:** middleware ProblemDetails, más FluentValidation, tests de integración (`WebApplicationFactory`), endpoint `GET /api/students/me/support-requests`, constancia PDF.

---

*Prefer progressive commits / Preferir commits progresivos (`feat:`, `fix:`, `test:`, `docs:`). Quality over quantity / Calidad sobre cantidad.*
