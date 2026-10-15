# Builder Journey Intelligence Platform

> **"Learn from the journey, not just the outcome."**

A modern SaaS web application that transforms publicly available, authorized social media data into structured, evidence-based journey reports. Users can discover how founders, indie hackers, and developers validate ideas, build prototypes, navigate pivots, overcome setbacks, and achieve documented milestones.

---

## 1. Product Philosophy & Key Differentiators

This is not a vanity social media metrics dashboard. The differentiator is **trustworthy, evidence-backed narrative reconstruction**:

1. **Strict Evidence Grounding**: Every documented milestone is connected to verifiable primary source posts, timestamps, and engagement snapshots.
2. **Explicit Fact vs. Inferred Deduction**: We distinguish between what the founder explicitly claimed and what the engine algorithmically inferred (e.g. engagement surges, demo media).
3. **No Fabricated Private Metrics**: We never invent or hallucinate private Stripe balances, churn rates, or retention curves. Stated figures are included only when publicly cited by the builder.
4. **Causality Honesty**: Statistical breakthroughs use disciplined language ("coincided with", "may have contributed to", "is associated with"). Social engagement is never falsely equated with business revenue.
5. **No Scraping**: All data ingest complies with official API terms (starting with X API v2) and authorized OAuth scopes with AES-256 token encryption at rest.

---

## 2. Technology Stack

### Backend
- **Framework**: ASP.NET Core 8 Web API (.NET 8 LTS)
- **Language**: C# 12
- **Data Access**: Entity Framework Core 8
- **Database**: PostgreSQL (production / Docker) with automatic SQLite support for zero-install local dev
- **Authentication**: JWT Bearer authentication with HMAC-SHA256 & BCrypt password hashing
- **Security**: AES-256 encryption service for OAuth tokens and sensitive credentials
- **Background Processing**: Hosted `BackgroundService` for scheduled data sync jobs
- **Documentation**: OpenAPI / Swagger UI with Bearer Token authorization
- **Automated Testing**: xUnit, FluentAssertions, Moq

### Frontend
- **Framework**: React 19 + TypeScript
- **Bundler**: Vite
- **Styling**: Tailwind CSS v4 (minimalist warm neutral editorial aesthetic)
- **Visualizations**: Recharts (cadence bar charts, category donut charts, resonance metrics)
- **Icons**: Lucide React
- **Routing**: React Router DOM v7

---

## 3. Architecture & Project Layout

```text
journey/
├── docker/
│   ├── Dockerfile.api            # Multi-stage .NET 8 build & runtime
│   ├── Dockerfile.web            # Multi-stage Node Vite + Nginx build
│   └── nginx.conf                # Nginx reverse proxy routing
├── docker-compose.yml            # Multi-container orchestration (PostgreSQL + API + Web)
├── src/
│   ├── Backend/
│   │   ├── BuilderJourney.sln
│   │   ├── BuilderJourney.Domain/          # Core entities, Enums, Value objects, Interfaces
│   │   ├── BuilderJourney.Application/     # DTOs, Services, Intelligence Engines:
│   │   │   ├── Engines/
│   │   │   │   ├── JourneyReconstructionEngine.cs   # Milestones & Evidence detection
│   │   │   │   ├── ContentStrategyAnalyzer.cs       # 10-category classification & DNA
│   │   │   │   ├── BreakthroughDetector.cs          # Inflection point detection
│   │   │   │   ├── PersonalizedActionPlanGenerator.cs # Stage-based roadmaps
│   │   │   │   └── BuilderComparator.cs             # Cross-builder comparison
│   │   ├── BuilderJourney.Infrastructure/  # EF Core DbContext, Providers, Background worker:
│   │   │   ├── Data/AppDbContext.cs
│   │   │   ├── Data/DatabaseSeeder.cs      # Curated builders & categories seeder
│   │   │   ├── Providers/XPlatformDataProvider.cs       # Official X v2 API provider
│   │   │   ├── Providers/CuratedPlatformDataProvider.cs # High-fidelity curated profiles
│   │   │   ├── Security/AesEncryptionService.cs
│   │   │   └── Background/BackgroundSyncWorker.cs
│   │   ├── BuilderJourney.Api/             # RESTful controllers, Swagger, middleware
│   │   └── BuilderJourney.Tests/           # Unit & Integration test suite
│   └── Frontend/                           # 12 React TypeScript Screens & UI components
│       ├── src/components/
│       │   ├── Navbar.tsx
│       │   ├── Footer.tsx
│       │   ├── TimelineView.tsx
│       │   ├── GrowthDashboard.tsx
│       │   ├── ContentDnaCard.tsx
│       │   ├── BreakthroughCard.tsx
│       │   └── EvidenceDrawer.tsx
│       └── src/pages/
│           ├── LandingPage.tsx
│           ├── LoginPage.tsx
│           ├── RegisterPage.tsx
│           ├── OnboardingPage.tsx
│           ├── DiscoveryPage.tsx
│           ├── BuilderProfilePage.tsx
│           ├── ComparisonPage.tsx
│           ├── ActionPlanPage.tsx
│           ├── WatchlistPage.tsx
│           └── SettingsPage.tsx
```

---

## 4. The 12 Implemented Application Screens

1. **Landing Page (`/`)**: Editorial hero communicating product manifesto, interactive teaser timeline, featured builder profiles, and 1-click instant demo access.
2. **Login (`/login`)**: Secure authentication with 1-click Demo Account login button (`demo@builderjourney.io` / `Password123!`).
3. **Register (`/register`)**: Registration with stage, objective, and weekly capacity calibration.
4. **Onboarding Wizard (`/onboarding`)**: Calibrates builder stage, primary 60-day goal, platform preference, and generates an initial roadmap.
5. **Builder Discovery (`/discover`)**: Searchable directory with category filter pills (SaaS, AI, DevTools, Open Source, Creator), audience sorting, and custom builder tracking.
6. **Builder Profile & Journey Report (`/builder/:id`)**: Executive summary, interactive timeline with confidence scores and evidence inspector, growth charts, lessons learned, and limitations disclosures.
7. **Growth Dashboard (Tab on Profile)**: Recharts cadence bar charts, category distribution donut, and ranked resonance posts.
8. **Content Strategy Analysis (Tab on Profile)**: Post category breakdown, format multipliers (e.g. video demos vs text), and discussion clusters.
9. **Builder Journey Comparator (`/compare`)**: Side-by-side alignment of up to 3 builders, comparative metrics matrix, and strategic divergence narrative.
10. **Personalized Action Plan (`/action-plan`)**: Weekly actionable experiment checklist with milestones linked to real builder precedents.
11. **Saved Builders Watchlist (`/saved`)**: Monitored founders with custom notes and latest milestone activity alerts.
12. **Settings & Governance (`/settings`)**: Profile calibration, X API v2 connection status, platform capability inspector, and background sync job audit logs.

---

## 5. Quickstart & Local Setup

### Option A: Local Development (Zero-Install SQLite)

The platform runs out-of-the-box using the built-in SQLite configuration and automatic database seeder:

#### 1. Start the ASP.NET Core Web API:
```bash
cd src/Backend/BuilderJourney.Api
dotnet run
```
The API starts on `http://localhost:5000` (or configured HTTPS port).
- Swagger UI documentation is available at `http://localhost:5000/swagger`.
- Default demo credentials automatically seeded: `demo@builderjourney.io` / `Password123!`.

#### 2. Start the React Frontend:
```bash
cd src/Frontend
npm install
npm run dev
```
The frontend launches on `http://localhost:5173`.

---

### Option B: Docker Compose (PostgreSQL + API + Web)

To run the complete production-ready stack in containers:
```bash
docker compose up --build
```
- Frontend Web App: `http://localhost`
- Backend API: `http://localhost:5000`
- PostgreSQL: `localhost:5432`

---

## 6. Running Automated Tests

Run the backend unit and integration test suite:
```bash
cd src/Backend
dotnet test
```

Verify frontend TypeScript compilation and bundling:
```bash
cd src/Frontend
npm run build
```

---

## 7. API Reference Overview

| Endpoint | Method | Description |
|---|---|---|
| `/api/v1/auth/register` | `POST` | Create a new user account |
| `/api/v1/auth/login` | `POST` | Authenticate and obtain JWT token |
| `/api/v1/auth/me` | `GET` | Get authenticated user profile |
| `/api/v1/builders` | `GET` | Filter, search, and paginate builder directory |
| `/api/v1/builders/{id}` | `GET` | Retrieve builder profile details |
| `/api/v1/builders/categories` | `GET` | List available builder categories |
| `/api/v1/builders/custom` | `POST` | Register a new public builder for tracking |
| `/api/v1/journey/{builderId}` | `GET` | Retrieve chronological timeline and evidence |
| `/api/v1/journey/{builderId}/reconstruct` | `POST` | Re-run algorithmic journey reconstruction |
| `/api/v1/content/{builderId}` | `GET` | Retrieve content strategy, cadence, and format DNA |
| `/api/v1/breakthroughs/{builderId}` | `GET` | Detect statistical inflection points and evidence |
| `/api/v1/comparison` | `POST` | Multi-builder comparative alignment matrix |
| `/api/v1/action-plans/generate` | `POST` | Synthesize stage-matched action plan |
| `/api/v1/action-plans/latest` | `GET` | Retrieve current user's active action plan |
| `/api/v1/action-plans/tasks/{taskId}` | `PATCH` | Update task completion status |
| `/api/v1/watchlist` | `GET` | Get saved builders watchlist |
| `/api/v1/watchlist` | `POST` | Add builder to watchlist |
| `/api/v1/watchlist/{builderId}` | `DELETE` | Remove builder from watchlist |
| `/api/v1/reports/builder/{builderId}` | `GET` | Get or generate full shareable journey report |
| `/api/v1/operations/capabilities` | `GET` | Query supported provider endpoints and limitations |
| `/api/v1/operations/sync-jobs` | `GET` | View audit log of background ingest operations |
