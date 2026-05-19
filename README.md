# Job Application Tracking

An ASP.NET Core MVC app for tracking job applications, built with Clean Architecture and DDD-lite.

## Architecture

```
JobApplicationTracking.Domain/          # Entities, repository interfaces
JobApplicationTracking.Application/     # Use case services, DTOs
JobApplicationTracking.Infrastructure/  # EF Core + SQLite, repository implementations
JobApplicationTracking/                 # MVC controllers, views, view models
```

## Running the App

```bash
dotnet run --project JobApplicationTracking
```

The SQLite database (`jobapplications.db`) is created automatically on first run and any pending migrations are applied on every startup.

## Database Migrations

When you add or change a field, create a new migration:

```bash
dotnet ef migrations add <MigrationName> \
  --project JobApplicationTracking.Infrastructure \
  --startup-project JobApplicationTracking
```

On next app startup, `Migrate()` applies any pending migrations automatically — no manual DB deletion needed, and existing data is preserved.

Migration files live in `JobApplicationTracking.Infrastructure/Persistence/Migrations/` and should be committed alongside the code change that caused the schema change.
