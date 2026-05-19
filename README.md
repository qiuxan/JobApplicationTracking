# Job Application Tracking

An ASP.NET Core MVC app for tracking job applications, built with Clean Architecture and DDD-lite.

## Architecture

```
JobApplicationTracking.Domain/          # Entities, repository interfaces
JobApplicationTracking.Application/     # Use case services, DTOs
JobApplicationTracking.Infrastructure/  # EF Core + SQLite, repository implementations
JobApplicationTracking/                 # MVC controllers, views, view models
```

## Features

- Create, view, edit, and delete job applications
- Filter by status (Applied, Phone Screen, Interview, Offer, Rejected, Withdrawn)
- Paste raw job posting text into a dedicated Description field
- Export all applications to CSV (dated filename, all fields included)
- Import from CSV — columns mapped by header name, so new fields are picked up automatically
- Duplicate detection on both manual create and CSV import (matched on Company + Role + Applied Date, case-insensitive)

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

## Duplicate Detection

A duplicate is defined as an existing record with the same **Company + Role + Applied Date** (case-insensitive).

- **Manual create**: blocked with an inline validation error on the form
- **CSV import**: duplicate rows are silently skipped and reported separately in the result message (e.g. _"3 imported, 2 duplicate(s) skipped"_)
