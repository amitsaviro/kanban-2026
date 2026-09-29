# Kanban Board Management System

A full-stack Kanban board application built in C# / .NET 6, featuring a WPF desktop frontend and a SQLite-backed business layer.

---

## Features

- **User authentication** — register, login, logout with email validation and password complexity rules
- **Board management** — create, delete, join, and leave shared boards; transfer ownership between members
- **Task workflow** — add tasks to Backlog, advance through In Progress to Done; set column limits
- **Task assignment** — assign and reassign tasks to board members with permission enforcement
- **Persistence** — all data survives restarts via SQLite (users, boards, columns, tasks, memberships)
- **In-progress view** — see all tasks assigned to you across every board you belong to

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Frontend | WPF (Windows Presentation Foundation), XAML, MVVM pattern |
| Business Logic | C# class library, Facade pattern, 3-layer architecture |
| Data Access | SQLite via `Microsoft.Data.Sqlite`, DTO + Repository pattern |
| Logging | log4net |
| Serialization | `System.Text.Json` |

---

## Architecture

```
Frontend (WPF / MVVM)
        ↓
  GradingService  ←  single entry point
        ↓
  Service Layer   ←  JSON in / JSON out
        ↓
 Business Layer   ←  Facades + Entities
        ↓
DataAccessLayer   ←  SQLite Controllers + DTOs
```

### Business Layer — Facade Pattern

Three facades coordinate the system:

- **`UserFacade`** — registration, login/logout, user lookup
- **`BoardFacade`** — board lifecycle, membership, column limits, global board registry
- **`TaskFacade`** — task creation, advancement, editing, assignment, in-progress queries

All three share a single `UserFacade` instance (injected by `GradingService`) so user state is consistent across operations.

### Database Schema

```sql
Users        (Email PK, Password)
Board        (Id PK, Name, OwnerEmail, NextTaskId)
BoardMembers (BoardId + UserEmail — composite PK)
Column       (BoardId + Ordinal — composite PK, Limit)
Task         (Id + BoardId — composite PK, Title, Description, DueDate, CreationTime, AssigneeEmail, ColumnOrdinal)
```

---

## Project Structure

```
├── Backend/
│   ├── ServiceLayer/       # JSON API wrappers (UserService, BoardService, TaskService, GradingService)
│   ├── BusinessLayer/      # Domain entities (User, Board, Column, Task) + Facades
│   └── DataAccessLayer/    # SQLite controllers + DTOs
├── Frontend/
│   ├── Views/              # XAML screens (Auth, BoardsList, Board, InProgressTasks)
│   ├── ViewModels/         # MVVM ViewModels
│   └── Models/             # Frontend data models
└── BackendTests/           # Acceptance test suite (31 tests)
```

---

## Running the Project

**Backend tests (Mac / Linux / Windows):**
```bash
dotnet run --project BackendTests
```

**Frontend (Windows only — WPF):**
```bash
dotnet run --project Frontend
```
Or open `Kanban.sln` in Visual Studio and set `Frontend` as the startup project.

---

## Key Design Decisions

- **Shared `UserFacade` instance** — all facades are wired together in `GradingService` so login state and user data are never split across isolated instances
- **Email normalization** — all emails are trimmed and lowercased at every entry point, making lookups case-insensitive throughout
- **Permission model** — Req 19 (advance = assignee only) is intentionally stricter than Req 20 (edit = assignee OR owner); each is enforced in its own method
- **Leave unassigns tasks** — when a member leaves a board, all their non-done assigned tasks are unassigned in both memory and DB atomically
- **Defensive copy on Members** — `Board.Members` returns `new List<string>(_members)` so callers cannot bypass `AddMember`/`RemoveMember`

---

## Authors

BGU Software Engineering — Introduction to Software Engineering, 2026
