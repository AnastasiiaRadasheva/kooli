#  KeelteKooli

A language course management app built with **C#** and **Windows Forms**. The project started as a graduation thesis (lõputöö), aimed at building a simple, reliable desktop app for managing language courses, teachers, and student enrollments — without the overhead of a full web or cloud setup.

##  What it does

Small schools and language course providers don't always need a big web platform — often a simple local app is enough. Kooli lets you:

- add, edit, and delete **language courses**
- manage **teacher** records
- view and manage **student enrollments** for each course
- do all of this through a straightforward Windows Forms interface, with data stored in a local database

## 🛠️ Tech stack

| Technology | Purpose |
|---|---|
| **C#** | application logic |
| **Windows Forms** | user interface |
| **Entity Framework Core** (Code First) | data model & database access |
| **LocalDB** | local database for development and testing |
| **EF Core Migrations** | database schema versioning |
| **Visual Studio** | development environment |

##  Architecture

The app follows a three-layer architecture to keep the code clean and maintainable:

```
Presentation (Windows Forms)
        ↓
Business Logic (services, validation)
        ↓
Data Access (DbContext, repositories)
```

Each layer sticks to its own job — the forms don't know anything about the database, and the business logic doesn't know anything about the forms. That makes the code easier to test and change.

## 🗂️ Data model

Three main entities:

- **Course** — name, language, level, description
- **Teacher** — first name, last name, email, phone
- **Enrollment** — student name, date, related course

One teacher can run multiple courses, and one course can have multiple enrollments.

##  Repo structure

```
├── Kool/          # application source code (Visual Studio project)
├── Docs/          # documentation
├── Kool.slnx      # Visual Studio solution file
└── loputoo.md     # thesis text (theory + practical description)
```

## Getting started

1. Open `Kool.slnx` in Visual Studio
2. Make sure LocalDB is installed
3. Run in the Package Manager Console:
   ```
   Update-Database
   ```
4. Run the app (F5)

##  Possible next steps

- A web version (ASP.NET Core) so multiple users can access it at once
- User roles and authentication (admin vs. student)
- Automated tests and CI/CD (e.g. GitHub Actions)
- Better database backup handling for production

---

**Author:** Anastasiia Radasheva
