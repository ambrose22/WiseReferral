# WiseReferral

An internal employee referral management tool built with ASP.NET Core 9. Employees submit candidate referrals, and the Talent team reviews, assigns, and progresses them through the hiring process.

**Live demo:** https://wisereferral.onrender.com

## Demo Credentials

| Role | Email | Password |
|------|-------|----------|
| Admin (Talent Ops / Talent Leads) | admin@referraltracker.com | Password123! |
| Recruiter (Talent Team) | recruiter@referraltracker.com | Password123! |
| Employee | employee@referraltracker.com | Password123! |

> The demo runs on Render's free tier and may take 30-60 seconds to wake up on first load. The database resets on each deploy, so test data won't persist.

## Features

- **Employee referral submission** — Any employee can refer a candidate with their details, location, justification, and an attached resume (PDF or DOCX)
- **Role-based access** — Employees see their own referrals, the Talent team sees everything, Admins manage assignments
- **Referral workflow** — Submitted → Screening → Interviewing → Hired/Declined, with a "More Info Requested" loop for follow-up questions
- **Assignment system** — Admins assign referrals to recruiters, which automatically moves them to Screening
- **Activity timeline** — Every status change, assignment, and comment is recorded with colour-coded badges
- **Resume management** — Upload and download candidate resumes
- **Smart routing** — Users land on the most relevant page after login based on their role

## Workflow

```
Employee submits referral
        ↓
   [Submitted]
        ↓
Admin assigns to Recruiter
        ↓
   [Screening]  ←→  [More Info Requested]
        ↓
  [Interviewing]
      ↓     ↓
  [Hired] [Declined]
```

## Tech Stack

- ASP.NET Core 9 MVC
- Entity Framework Core 9 with SQLite
- ASP.NET Core Identity (authentication and role-based authorisation)
- Bootstrap 5
- Docker (for Render deployment)

## Running Locally

```bash
# Clone the repo
git clone https://github.com/ambrose22/WiseReferral.git
cd WiseReferral

# Run the app
dotnet run
```

The app creates the database and seeds test users automatically on first run.

## Project Structure

```
Controllers/          — Thin controllers with role-based authorisation
Data/                 — EF Core DbContext and database configuration
Models/               — Entity models, enums, and view models
Services/             — Business logic (referral CRUD, workflow transitions)
Views/                — Razor views with Bootstrap 5
Migrations/           — EF Core database migrations
```

## Future Plans

This is a standalone prototype designed to eventually integrate as a module within [WiseRecruiter](https://github.com/alexfinch1992/WiseRecruiter), connecting referrals to live job postings and candidate pipelines.
