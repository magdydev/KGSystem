# KGSystem

A full-stack **Kindergarten Management System**: a **.NET 8** API built with **Domain-Driven Design / Clean Architecture**, **SQL Server** via EF Core, and an **Angular** (standalone components) frontend. Bilingual (English/Arabic, RTL) with role-based staff access.

It started life as a generic full-stack template — that lineage is still visible in the `Product` sample aggregate, the `KGSystem.API/Extensions` scaffolding, and the `New-ProjectFromTemplate.ps1` rename script — but the repo now implements a real application: child enrollment, attendance, monthly fee payments, and academic-year/phase administration for a kindergarten/nursery.

## What it does

- **Children** — CRUD for child records: bilingual (AR/EN) names, guardian contact info, nationality, address, photo, status.
- **Enrollments** — link a child to a KG phase and academic year.
- **Attendance** — record daily attendance per child, batch-record a whole day at once, view "today", and pull historical reports (exportable — see Reports below).
- **Payments** — record/update monthly fee payments per enrollment, view payment history per child.
- **Reference data** — admin-managed **KG Phases** (e.g. KG1/KG2), **Academic Years**, and **Monthly Fees** (per year/month, in EGP by default).
- **Dashboard** — summary view for managers.
- **Branding** — app name, logo, and colors are database-backed and editable via the API, not build-time constants.
- **Auth** — JWT login with two roles, **Manager** and **Accountant**, enforced on both API (`[Authorize(Roles = ...)]`) and frontend (route guards + nav filtering).

## Solution layout

```
KGSystem/
├── backend/
│   ├── KGSystem.sln
│   ├── Directory.Build.props        # shared compiler settings (nullable, implicit usings, ...)
│   ├── global.json                  # pins the .NET SDK version
│   ├── src/
│   │   ├── KGSystem.Domain/         # entities, value objects, domain events, repository interfaces
│   │   ├── KGSystem.Application/    # CQRS commands/queries per feature, DTOs, validators, AutoMapper profiles
│   │   ├── KGSystem.Infrastructure/ # EF Core DbContext, repositories, UnitOfWork, migrations, seed data
│   │   └── KGSystem.API/            # ASP.NET Core Web API, Identity/JWT auth, Swagger, DI composition root
│   └── tests/
│       └── KGSystem.Tests/          # xUnit test project (scaffolded, no test cases written yet — see Tests below)
├── frontend/                        # Angular app (standalone components, lazy-loaded features, EN/AR i18n)
├── scripts/
│   └── New-ProjectFromTemplate.ps1  # rebrands backend + frontend for a new project (see below)
├── docker-compose.yml                # SQL Server + API for local development
└── README.md
```

### Backend feature folders

`KGSystem.Application` and the matching `KGSystem.API/Controllers` are organized per feature, each following the same `Commands/<UseCase>`, `Queries/<UseCase>`, `Dtos` shape:

- `Attendance` (`RecordAttendance`, `BatchRecordAttendance`, `UpdateAttendance`, `GetAttendance`, `GetTodayAttendance`)
- `Children` (`CreateChild`, `UpdateChild`, `DeleteChild`, `GetChildById`, `GetChildren`)
- `Enrollments` (`CreateEnrollment`, `UpdateEnrollment`, `GetEnrollments`, `GetChildEnrollments`)
- `Payments` (`RecordPayment`, `UpdatePayment`, `GetPayments`, `GetChildPayments`)
- `ReferenceData/AcademicYears`, `ReferenceData/KGPhases`, `ReferenceData/MonthlyFees`
- `Dashboard` (`GetDashboardSummary`)
- `Branding` (`UpdateBranding`, `GetBranding`)

### Why this structure

- **Domain** has zero package dependencies — it's pure C#. EF Core/ASP.NET Core logic belongs in Infrastructure or the API instead.
- **Application** orchestrates use cases (`ICommandHandler<,>` / `IQueryHandler<,>`) but never talks to a database directly — it depends only on repository *interfaces* defined in Domain.
- **Infrastructure** implements those interfaces with EF Core and SQL Server.
- **API** is the thin composition root: controllers translate HTTP to commands/queries via a small `IDispatcher`, nothing more.

## Backend

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (local install, `docker compose up sqlserver`, or LocalDB on Windows)

### Run locally

Migrations are already committed under `KGSystem.Infrastructure/Persistence/Migrations` — you don't need to generate an initial one.

```bash
cd backend
dotnet restore
dotnet build

# Apply migrations — the API also does this automatically at startup in Development
dotnet ef database update \
  --project src/KGSystem.Infrastructure \
  --startup-project src/KGSystem.API

dotnet run --project src/KGSystem.API
```

The API starts at `https://localhost:5101` (see `src/KGSystem.API/Properties/launchSettings.json`) with Swagger UI at `/swagger`. On first run in Development it applies migrations and seeds roles, sample users, reference data, and sample children/enrollments/payments (see `ApplicationDbContextSeed`).

> `dotnet ef` not installed? Run `dotnet tool install --global dotnet-ef` first.

### Configuration

Connection strings, JWT settings, CORS origins, and Serilog sinks all live in `src/KGSystem.API/appsettings.json` / `appsettings.Development.json`. Serilog writes to console and to `logs/log-.txt` (daily rolling, 14-day retention).

**Never commit real secrets.** The committed `Jwt:SigningKey` is a placeholder — for anything beyond local development, use `dotnet user-secrets`, environment variables, or a secret manager to override `Jwt:SigningKey` / `ConnectionStrings:DefaultConnection`.

### Authentication & roles

JWT auth is fully wired, not just scaffolding: `AuthController` issues tokens.

- `POST /api/v1/auth/login` — `{ username, password }` → `{ token, email, roles }`. Anonymous.
- `POST /api/v1/auth/register` — `{ username, password, role }` → same shape. **Requires the `Manager` role** — there's no public signup, so bootstrap with one of the seeded accounts below (or seed your own) before creating further users.

Two roles gate the rest of the API: **`Manager`** (full access, including dashboard, reference-data admin, branding, settings, user registration) and **`Accountant`** (day-to-day operations: children, enrollments, attendance, payments — but not reference-data writes, dashboard, or branding). Check each controller's `[Authorize(Roles = ...)]` attributes for the exact split.

The Development seed (`ApplicationDbContextSeed`) creates these accounts if none exist — **change or remove them before any non-local deployment**:

| Username     | Password       | Roles                  |
|--------------|----------------|-------------------------|
| `admin`      | `Admin@123`    | Manager, Accountant     |
| `manager`    | `Manager@123`  | Manager                 |
| `accountant` | `Accountant@123` | Accountant            |

### API surface

All routes are versioned under `api/v1/`. Base route is the plural controller name unless noted.

| Controller | Base route | Notes |
|---|---|---|
| `AuthController` | `/auth` | `login` (anonymous), `register` (Manager) |
| `ChildrenController` | `/children` | list/get: Accountant+Manager; create/update/delete: Accountant |
| `EnrollmentsController` | `/enrollments` | list/get-by-child: Accountant+Manager; create/update: Accountant |
| `AttendanceController` | `/attendance` | list/today: Accountant+Manager; record/batch/update: Accountant |
| `PaymentsController` | `/payments` | list/get-by-child: Accountant+Manager; record/update: Accountant |
| `AcademicYearsController` | `/academicyears` | list: Accountant+Manager; create: Manager |
| `KGPhasesController` | `/kgphases` | list: Accountant+Manager; create/update: Manager |
| `MonthlyFeesController` | `/monthlyfees` | list/get-by-year: Accountant+Manager; patch-by-year: Manager |
| `DashboardController` | `/dashboard/summary` | Manager |
| `SettingsController` (file: `BrandingController.cs`) | `/settings/branding` | GET anonymous; PUT: Manager |
| `HealthController` | `/health` | anonymous |

### Adding a new feature

1. **Domain**: add the entity/value object under `KGSystem.Domain/Entities` (or `ValueObjects`), plus an `I<Entity>Repository` interface if it needs custom queries.
2. **Infrastructure**: add an `IEntityTypeConfiguration<T>` under `Persistence/Configurations`, a repository implementation under `Persistence/Repositories`, and expose it on `IUnitOfWork`.
3. **Application**: add a feature folder (see the list above) with `Commands/<UseCase>`, `Queries/<UseCase>`, a validator, and a handler. DI registration is automatic — `DependencyInjection.AddApplication()` scans the assembly for handler implementations.
4. **API**: add a controller (or actions on an existing one) that calls `IDispatcher.Send(...)`, with `[Authorize(Roles = ...)]` matching the access level the feature needs.
5. **Frontend**: add `src/app/features/<feature>/` with its own routes file, a service in `core/services`, and a nav entry in `SidebarComponent` (with `roles` if it should be role-gated).

### Tests

```bash
cd backend
dotnet test
```

`KGSystem.Tests` is scaffolded (xUnit, references Domain and Application) but currently has **no test files** — it's an empty project ready for tests, not existing coverage.

## Frontend (Angular)

### Prerequisites

- Node.js 20+ and npm

### Run locally

```bash
cd frontend
npm install
npm start        # ng serve, http://localhost:4200
```

`npm run build` / `npm run build:prod` produce a production build in `dist/`. `npm test` runs the Karma/Jasmine unit tests, `npm run lint` runs ESLint, `npm run format` runs Prettier.

### Structure

```
src/app/
├── core/
│   ├── guards/           # auth.guard.ts (requires a valid session), role.guard.ts (requires specific roles)
│   ├── interceptors/     # auth.interceptor.ts (attaches JWT), error.interceptor.ts (401/HTTP errors),
│   │                     # loading.interceptor.ts (drives the global spinner)
│   ├── models/           # one model per domain concept (child, enrollment, attendance, payment, ...)
│   └── services/         # one HTTP service per feature, plus auth/branding/language/loading/toast/app-log
├── shared/
│   ├── components/       # app-header, app-footer, sidebar, toast, modal, global-spinner, date-input
│   └── layouts/           # auth-layout (login screen), main-layout (sidebar + header shell)
├── features/
│   ├── auth/login/
│   ├── dashboard/
│   ├── children/          (list, detail, form)
│   ├── enrollments/        # via child detail/enrollment flows
│   ├── attendance/        (list, batch/today, history report)
│   ├── payments/          (list, form)
│   ├── reference/         (phase, academic-year, monthly-fee — list + form each)
│   ├── settings/          # branding editor
│   └── products/          # original template sample feature, still present
├── app.config.ts          # provideHttpClient, provideRouter, provideTranslateService, interceptor registration
└── app.routes.ts          # top-level route table, with authGuard / roleGuard(['Manager']) per route
```

Routing follows one pattern throughout: every feature route (other than `auth`) is gated by `authGuard`; manager-only areas (`dashboard`, `phases`, `academic-years`, `monthly-fees`, `settings`) add `roleGuard(['Manager'])`. `SidebarComponent` filters its own nav items the same way, reading `roles` off each `NavItem` and checking `authService.hasRole(role)`.

Services talk to the API through `HttpClient` and return Observables — consume them in templates with the `async` pipe rather than manual `subscribe()`. The API base URL is set per environment in `src/environments/environment.ts` (dev, `http://localhost:5100/api`) and `environment.production.ts` (prod build).

### Reports

`src/assets/templates/attendance-register.html` and `child-attendance-report.html` are print/export templates used by the attendance history feature; `html2canvas` + `jspdf` (frontend dependencies) render them to PDF client-side.

### Branding (name / logo / colors — database-backed)

Name, logo URL, and primary/secondary colors are **not** build-time constants — they're a row in the database, served by `SettingsController` (`GET /api/v1/settings/branding` — anonymous, `PUT /api/v1/settings/branding` — Manager) and read by the `BrandingSetting` aggregate (`KGSystem.Domain/Entities/BrandingSetting.cs`).

- **Backend**: seeded once at startup (`ApplicationDbContextSeed`) with the defaults in `BrandingDefaults` (`KGSystem.Domain/Entities/BrandingDefaults.cs`), including a currency default (EGP). Colors must be valid hex (`#RRGGBB` or `#RGB`), enforced both by the domain entity and the command validator.
- **Frontend**: `BrandingService` (`core/services/branding.service.ts`) fetches `/api/v1/settings/branding` once at startup via an `APP_INITIALIZER` in `app.config.ts`, then sets `document.title` and the `--color-primary`/`--color-secondary` CSS custom properties on `<html>`. If the API is unreachable, it falls back to the same defaults as the backend seed rather than blocking startup.
- `AppHeaderComponent` renders `brandingService.branding().appName` / `.logoUrl` directly.
- **"Powered by" footer**: `AppFooterComponent` shows `src/assets/magdytech-logo.png` with a `FOOTER.POWERED_BY` translated caption. This is agency attribution, entirely separate from the database-backed branding above — it's a static asset, not a setting, and the rename script skips it deliberately.

### Internationalization (English / Arabic)

Built with [`@ngx-translate/core`](https://github.com/ngx-translate/core). Translation files live in `src/assets/i18n/en.json` and `ar.json`; add a key there and reference it in a template with the `translate` pipe.

`LanguageService` (`core/services/language.service.ts`) owns the active language: it persists the choice to `localStorage`, and sets `<html lang>`/`<html dir>` so Arabic renders right-to-left automatically via native CSS direction. `AppHeaderComponent` has an EN/AR switcher wired to it; call `languageService.use('ar' | 'en')` from anywhere else you need to change language programmatically.

To add a third language: create `src/assets/i18n/<code>.json`, add `<code>` to `SUPPORTED_LANGUAGES` in `language.service.ts` (and to `RTL_LANGUAGES` too, if it's right-to-left), and add a button to `AppHeaderComponent`.

## Docker (local SQL Server + API)

```bash
docker compose up --build
```

This starts SQL Server on `localhost:1433` and the API on `localhost:5100`. Run the Angular dev server separately with `npm start` in `frontend/` — its dev-server proxy is not wired into `docker-compose.yml` by design, so you get fast rebuilds while iterating on the UI.

## Starting a new project from this template

The rename script from the original template is still here, for the case where you want to fork this repo as a starting point for something else entirely:

```bash
pwsh ./scripts/New-ProjectFromTemplate.ps1 -NewName Acme          # rename everything to Acme.*
pwsh ./scripts/New-ProjectFromTemplate.ps1 -NewName Acme -WhatIf  # preview first, changes nothing
```

It renames every `KGSystem` / `kgsystem` / `kg-system` occurrence — namespaces, `.csproj`/`.sln` files and folders, the Angular package name, `appsettings.json`, `docker-compose.yml`, i18n strings — and regenerates the API project's `UserSecretsId`. It does **not** touch the "Powered by MagdyTech Solutions" footer. It also does **not** remove the kindergarten-specific domain logic (Children, Attendance, Payments, etc.) — it only rebrands names, so a genuinely different project should treat this as a reference implementation to copy patterns from rather than a blank slate.

## Known gaps

- **No test coverage** — `KGSystem.Tests` is an empty scaffold (see Tests above).
- **Seeded auth accounts are dev-only** — rotate or remove the `admin` / `manager` / `accountant` users and their passwords before deploying anywhere reachable.
- **`register` has no public entry point** — by design (an unauthenticated attacker can't create accounts), but it means a fresh environment needs the seed step (or a manually-created admin) before anyone else can log in.
