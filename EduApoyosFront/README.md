# EduApoyos Frontend / Frontend EduApoyos

Angular 17 SPA for managing higher-education economic support requests (scholarships, credits, subsidies).

SPA Angular 17 para la gestión de solicitudes de apoyo económico (becas, créditos, subsidios) en educación superior.

| | |
|---|---|
| **Stack** | Angular 17+ · Angular Material · JWT · jsPDF |
| **API** | .NET 8 EduApoyos Backend (`/api`) |
| **Roles** | Advisor (Asesor) · Student (Estudiante) |

---

# English

## 1. Description

EduApoyos Frontend is a single-page application that consumes the EduApoyos REST API:

- **Advisors** manage students and support requests (list, filters, status changes with mandatory observation and history).
- **Students** use a self-service portal: profile, own requests, create request, download constancy (TXT / PDF).

### Functional coverage (UI)

| ID | Feature |
|----|---------|
| RF-01 | Login with JWT; role-based navigation |
| RF-02 | Student list and create dialog (document type + academic program selects) |
| RF-03 | Support requests list/detail; status flow; final states locked |
| RF-04 | Student portal + constancy download (text and PDF) |
| RF-05 | Filters by status/type and pagination |

---

## 2. Project structure

```
EduApoyosFront/
├── Dockerfile
├── nginx.conf
├── docker-compose.yml
├── src/
│   ├── environments/
│   │   ├── environment.ts
│   │   ├── environment.development.ts
│   │   └── environment.production.ts
│   └── app/
│       ├── app.config.ts
│       ├── app.routes.ts
│       ├── core/
│       │   ├── models/
│       │   ├── services/
│       │   ├── guards/
│       │   └── interceptors/
│       └── features/
│           ├── auth/login/
│           ├── advisor/
│           ├── support-requests/
│           └── student/
└── README.md
```

---

## 3. Prerequisites

- Node.js **18.x or 20.x** LTS  
- npm  
- EduApoyos API running (local or Docker)  
- (Optional) Docker Desktop  

```bash
node -v
npm -v
```

---

## 4. Local installation and run

```bash
cd EduApoyosFront
npm install
npx ng serve
# or: npm start
```

Open **http://localhost:4200**

### API base URL

`src/environments/environment.ts` / `environment.development.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:8080/api'   // API in Docker
  // apiUrl: 'https://localhost:7174/api' // local Kestrel HTTPS
};
```

CORS on the API must allow `http://localhost:4200`.

---

## 5. Demo users (API seed)

| Role | Email | Password |
|------|-------|----------|
| Advisor | advisor@eduapoyos.com | Advisor123* |
| Student | student@eduapoyos.com | Student123* |

---

## 6. Main routes

| Path | Access |
|------|--------|
| `/login` | Public |
| `/advisor` | Advisor |
| `/support-requests` | Advisor |
| `/support-requests/:id` | Advisor or owner Student |
| `/student` | Student |

---

## 7. Docker — run the frontend in a container

### Required files in `EduApoyosFront/`

- `Dockerfile`
- `nginx.conf`
- `docker-compose.yml` (optional helper)

### Production environment

`src/environments/environment.production.ts`:

```typescript
export const environment = {
  production: true,
  apiUrl: 'http://localhost:8080/api'
};
```

The **browser** calls the API on the host (`localhost:8080`), not the Docker internal service name.

### Start API first

```bash
cd EduApoyosBack
docker compose up -d
```

### Build and run frontend

```bash
cd EduApoyosFront
docker compose up --build -d
```

| Service | URL |
|---------|-----|
| Frontend | http://localhost:4200 |
| API (separate) | http://localhost:8080 |

Manual:

```bash
docker build -t eduapoyos-front .
docker run --rm -p 4200:80 eduapoyos-front
```

### Useful commands

```bash
docker compose logs -f frontend
docker compose down
```

### Dockerfile notes

Angular 17+ often outputs to `dist/EduApoyosFront/browser`. If `COPY` fails, list `dist/` after `npm run build` and change the path to `dist/EduApoyosFront` without `/browser`.

---

## 8. Production build (without Docker)

```bash
npm run build -- --configuration=production
```

Artifacts under `dist/EduApoyosFront/`.

---

## 9. Technical notes

- JWT interceptor attaches `Authorization: Bearer` on HTTP calls.
- Guards: `authGuard`, `roleGuard`.
- Student portal uses `GET /api/students/me` (no manual Student Id).
- Status **Approved** / **Rejected** cannot be changed again (Advisor UI locked).
- Observation is **required** when changing status.
- Material select overlays: ensure `.cdk-overlay-container { z-index: 2000; }` if dialogs cover the panel.
---

# Español

## 1. Descripción

EduApoyos Frontend es una aplicación de una sola página que consume la API REST de EduApoyos:

- **Asesores** gestionan estudiantes y solicitudes de apoyo (listado, filtros, cambio de estado con observación obligatoria e historial).
- **Estudiantes** usan un portal de autogestión: perfil, sus solicitudes, crear solicitud y descargar constancia (TXT / PDF).

### Cobertura funcional (UI)

| ID | Funcionalidad |
|----|----------------|
| RF-01 | Login con JWT; navegación por rol |
| RF-02 | Listado y alta de estudiantes (selects de tipo documento y programa) |
| RF-03 | Listado/detalle de solicitudes; flujo de estados; estados finales bloqueados |
| RF-04 | Portal del estudiante + constancia (texto y PDF) |
| RF-05 | Filtros por estado/tipo y paginación |

---

## 2. Estructura del proyecto

```
EduApoyosFront/
├── Dockerfile
├── nginx.conf
├── docker-compose.yml
├── src/
│   ├── environments/
│   └── app/
│       ├── core/          # models, services, guards, interceptors
│       └── features/      # login, advisor, support-requests, student
└── README.md
```

---

## 3. Prerrequisitos

- Node.js **18.x o 20.x** LTS  
- npm  
- API EduApoyos en ejecución (local o Docker)  
- (Opcional) Docker Desktop  

```bash
node -v
npm -v
```

---

## 4. Instalación y ejecución local

```bash
cd EduApoyosFront
npm install
npx ng serve
# o: npm start
```

Abrir **http://localhost:4200**

### URL del API

`src/environments/environment.ts` / `environment.development.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:8080/api'   // API en Docker
  // apiUrl: 'https://localhost:7174/api' // Kestrel local HTTPS
};
```

El API debe permitir CORS desde `http://localhost:4200`.

---

## 5. Usuarios de demostración (seed del API)

| Rol | Email | Contraseña |
|-----|-------|------------|
| Advisor | advisor@eduapoyos.com | Advisor123* |
| Student | student@eduapoyos.com | Student123* |

---

## 6. Rutas principales

| Ruta | Acceso |
|------|--------|
| `/login` | Público |
| `/advisor` | Asesor |
| `/support-requests` | Asesor |
| `/support-requests/:id` | Asesor o estudiante dueño |
| `/student` | Estudiante |

---

## 7. Docker — levantar el frontend en contenedor

### Archivos necesarios en `EduApoyosFront/`

- `Dockerfile`
- `nginx.conf`
- `docker-compose.yml` (opcional)

### Entorno de producción

`src/environments/environment.production.ts`:

```typescript
export const environment = {
  production: true,
  apiUrl: 'http://localhost:8080/api'
};
```

El **navegador** llama al API en el host (`localhost:8080`), no al nombre interno del servicio Docker.

### Primero el API

```bash
cd EduApoyosBack
docker compose up -d
```

### Construir y ejecutar el frontend

```bash
cd EduApoyosFront
docker compose up --build -d
```

| Servicio | URL |
|----------|-----|
| Frontend | http://localhost:4200 |
| API (aparte) | http://localhost:8080 |

Manual:

```bash
docker build -t eduapoyos-front .
docker run --rm -p 4200:80 eduapoyos-front
```

### Comandos útiles

```bash
docker compose logs -f frontend
docker compose down
```

### Nota del Dockerfile

Angular 17+ suele generar `dist/EduApoyosFront/browser`. Si falla el `COPY`, revisa `dist/` tras el build y usa la ruta sin `/browser` si aplica.

---

## 8. Build de producción (sin Docker)

```bash
npm run build -- --configuration=production
```

Salida en `dist/EduApoyosFront/`.

---

## 9. Notas técnicas

- Interceptor JWT agrega `Authorization: Bearer` en las peticiones.
- Guards: `authGuard`, `roleGuard`.
- Portal estudiante: `GET /api/students/me` (sin pedir Student Id a mano).
- Estados **Approved** / **Rejected** no se pueden volver a cambiar.
- Observación **obligatoria** al cambiar estado.
- Si el panel del `mat-select` queda detrás de un modal: `.cdk-overlay-container { z-index: 2000; }`.

---

