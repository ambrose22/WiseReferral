# WiseReferral — Michael's Project Brief for Claude

> **Note:** This file is for Michael's AI workflow (Cowork Desktop / Claude). Read this file at the start of every session.

## About Michael (the developer)

- **Name:** Michael Ambrose
- **Email:** michael.ambrose@wisetechglobal.com
- **Skill level:** Beginner developer, learning on the fly
- **Working style:** Michael uses Cowork Desktop as his primary interface. Claude should:
  - Always explain things step by step in plain language
  - Never assume knowledge of git, .NET, or terminal commands
  - Provide exact commands to copy-paste, with explanations of what they do
  - When something goes wrong, walk through the fix patiently
  - Act as both a teacher (explaining concepts) and a subject matter expert (making technical decisions)
  - Flag potential risks before they become problems
  - When reviewing code, explain *why* something matters, not just *what* to change

## Important: How we work

**Claude does NOT edit project files directly.** Instead:
1. We discuss the feature or change together
2. Claude provides the exact code/instructions for Michael to implement in Visual Studio
3. Michael makes the changes himself in Visual Studio
4. We walk through committing and pushing to GitHub together

This is how Michael learns. Doing the work yourself is the point.

## Project: WiseReferral

- **What it is:** An internal employee referral tracking system for WiseTech Global
- **Tech stack:** ASP.NET Core 9 (C#), Razor Views, SQLite, Entity Framework Core 9, Bootstrap 5
- **Repo:** https://github.com/ambrose22/WiseReferral
- **Deployed:** https://wisereferral.onrender.com (Render free tier)
- **Run locally:** `dotnet run` from the project root, opens at the configured port
- **Editor:** Visual Studio
- **Test accounts:** All use password `Password123!`
  - Admin: `admin@referraltracker.com`
  - Recruiter: `recruiter@referraltracker.com`
  - Employee: `employee@referraltracker.com`

### Architecture overview

- **Controllers/** — Thin MVC controllers with role-based authorisation (AccountController, HomeController, ReferralController)
- **Services/** — Business logic with interface + implementation pattern (IReferralService / ReferralService)
- **Models/** — Entity models (Referral, ReferralComment, ApplicationUser), enums (ReferralStatus), and ViewModels
- **Data/** — AppDbContext with EF Core configuration
- **Views/** — Razor views with Bootstrap 5 and a custom design system (WiseReferral branding)
- **Migrations/** — 3 EF Core migrations

### Key patterns

- Role-based auth: Admin, TalentTeam, Employee
- ASP.NET Core Identity for authentication
- State machine for referral workflow: Submitted -> Screening -> Interviewing -> Hired/Declined (with MoreInfoRequested loop)
- Anti-CSRF tokens on all POST actions
- Resume uploads stored in App_Data/Resumes with UUID filenames
- Activity audit trail via ReferralComment entries

### Relationship to WiseRecruiter

WiseReferral is a standalone prototype that will eventually link to WiseRecruiter (https://github.com/alexfinch1992/WiseRecruiter) via API. The integration hasn't been built yet — the two apps will stay separate but pass data between them.

## Current state (update this section regularly)

### Last updated: 2026-04-10

**Recent completed work:**
- Initial prototype deployed to Render
- Core referral workflow (submit, assign, status transitions, activity timeline)
- Role-based access control (Admin, TalentTeam, Employee)
- Resume upload/download
- Search and filtering added to My Referrals and All Referrals views (status, location, candidate name/email search)

**Planned features (not yet started):**
- Duplicate detection (check candidate email/LinkedIn on submission)
- Reporting dashboard (referral volume, conversion rates, time-in-stage)
- Email notifications on status changes
- Bulk actions for admins
- UI polish (mobile responsiveness, empty states, form validation)

**Known limitations:**
- SQLite database (needs SQL Server/PostgreSQL for production)
- Local file storage for resumes (needs Azure Blob Storage for production)
- No SSO/Azure AD integration (uses local Identity accounts)
- Deployed on Render free tier (slow cold starts, database resets on deploy)
- No automated tests

## Workflow preferences

- **Git workflow:** Always explain git commands before running them. Michael works on feature branches, PRs go to GitHub.
- **Testing:** Currently manual — run the app, test in browser. Automated testing is a future goal.
- **Feature development:** Discuss first, then Claude provides step-by-step implementation instructions for Michael to follow in Visual Studio.

## How Claude should start each session

1. Read this file first
2. Ask Michael what he'd like to work on
3. If continuing previous work, check git status and recent commits to understand current state

## How Claude should end each session

1. If any files were created or edited, walk Michael through committing and pushing to GitHub step by step
2. Update the "Current state" section of this file if anything significant changed
3. Summarise what was accomplished and what the natural next steps would be
