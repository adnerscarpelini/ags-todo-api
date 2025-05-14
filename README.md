# ags-todo-api

A simple To-Do List API built with C# and .NET 8, created as a study project to explore and implement core concepts of API development, including JWT authentication, Entity Framework Core, and RESTful principles.

## Table of Contents

-   [Project Overview](#project-overview)
-   [Features](#features)
-   [Technologies Used](#technologies-used)
-   [NuGet Packages](#nuget-packages)
-   [Why SQLite?](#why-sqlite)
-   [Project Structure](#project-structure)
-   [Setup and Running the Project](#setup-and-running-the-project)
-   [API Endpoints](#api-endpoints)

## Project Overview

`ags-todo-api` provides a backend service for managing tasks. Users can register, log in, and then create, retrieve, update, and delete their personal to-do items. The primary goal of this project was to practice building a secure and well-structured API using modern .NET technologies.

This project serves as a demonstration of skills in:
* RESTful API design
* JWT-based authentication and authorization
* Data persistence with Entity Framework Core
* Repository pattern for data access
* Dependency Injection
* API documentation with Swagger/OpenAPI

## Features

* User registration and login.
* JWT token-based authentication.
* CRUD operations for Tasks (Create, Read, Update, Delete).
* Tasks are user-specific (users can only access their own tasks).
* Password hashing using BCrypt.
* API documentation via Swagger UI.

## Technologies Used

* **.NET 8**
* **ASP.NET Core Web API 8**
* **C#**
* **Entity Framework Core 8** (ORM for database interaction)
* **SQLite** (Database)
* **JWT (JSON Web Tokens)** (For securing API endpoints)
* **Swagger/OpenAPI** (For API documentation and testing)
* **BCrypt.Net-Next** (For password hashing)

## NuGet Packages

This project relies on the following key NuGet packages:

* `Microsoft.AspNetCore.Authentication.JwtBearer`: For JWT authentication middleware.
* `Microsoft.EntityFrameworkCore.Sqlite`: EF Core provider for SQLite.
* `Microsoft.EntityFrameworkCore.Design`: For EF Core design-time tools (e.g., migrations).
* `Microsoft.EntityFrameworkCore.Tools`: For EF Core command-line tools (`dotnet ef`).
* `Swashbuckle.AspNetCore`: For Swagger/OpenAPI documentation.
* `System.IdentityModel.Tokens.Jwt`: For creating JWT tokens (often a transitive dependency but good to note).
* `BCrypt.Net-Next`: For hashing user passwords.

## Why SQLite?

SQLite was chosen as the database for this project due to its simplicity and ease of use, especially for development and learning purposes:
* **Serverless:** No separate database server process needs to be installed or managed.
* **File-based:** The entire database is contained in a single file (`agsTodo.db` in this project), making it highly portable and easy to set up.
* **Lightweight:** It's fast and efficient for small to medium-sized applications.
* **Good EF Core Support:** Well-supported by Entity Framework Core.
* **Focus on API Logic:** Allows concentration on the API development logic rather than complex database administration.

For a production environment with higher concurrency or scalability needs, a more robust database server like PostgreSQL, SQL Server, or MySQL would typically be considered.

## Project Structure

The project follows a standard structure for ASP.NET Core Web APIs:
```plaintext
ags-todo-api/
├── Controllers/        # API controllers handling HTTP requests
│   ├── AuthController.cs   # Handles user registration and login
│   └── TaskController.cs   # Handles CRUD operations for tasks
├── Data/               # Data access layer components
│   ├── Repositories/   # Repository pattern implementation (e.g., TaskRepository.cs)
│   └── TodoDbContext.cs  # Entity Framework Core DbContext
├── DTOs/               # Data Transfer Objects for API requests/responses
│   ├── LoginResponseDto.cs
│   ├── TaskCreateDto.cs
│   ├── TaskUpdateDto.cs
│   ├── UserLoginDto.cs
│   └── UserRegisterDto.cs
├── Migrations/         # EF Core database migration files
├── Models/             # Domain models/entities
│   ├── TaskModel.cs
│   └── UserModel.cs
├── Services/           # Business logic services
│   └── TokenService.cs   # Service for generating JWT tokens
├── Properties/         # Project properties (e.g., launchSettings.json)
├── agsTodo.db          # SQLite database file (created after migrations)
├── ags-todo-api.http   # File for making HTTP requests (e.g., for VS Code REST Client)
├── appsettings.json    # Application configuration (connection strings, JWT settings)
└── Program.cs          # Main application entry point, service and middleware configuration
```

* **`/Controllers`**: Contains API controllers that handle incoming HTTP requests and return responses (e.g., `AuthController`, `TaskController`).
* **`/Data`**: Includes the `TodoDbContext` for EF Core and the `Repositories` subfolder which contains repository classes (like `TaskRepository`) that implement the data access logic, abstracting EF Core details from controllers.
* **`/DTOs`**: Holds Data Transfer Objects, which are simple classes used to shape data for API requests and responses, separating the API contract from internal domain models.
* **`/Migrations`**: Stores Entity Framework Core migration files, which track changes to the database schema.
* **`/Models`**: Defines the domain entities (`TaskModel`, `UserModel`) that represent the application's core data structures and are mapped to database tables.
* **`/Services`**: Contains business logic services. In this project, `TokenService` is responsible for JWT generation.
* **`Program.cs`**: The main entry point of the application. In .NET 6+ (and .NET 8), it's used to configure services, the HTTP request pipeline (middleware), and run the application.
* **`appsettings.json`**: Contains configuration settings for the application, such as the database connection string and JWT parameters.
* **`agsTodo.db`**: The actual SQLite database file, created when EF Core migrations are applied.

## Setup and Running the Project

To set up and run this project locally, follow these steps:

1.  **Prerequisites:**
    * [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later installed.
    * A Git client to clone the repository.

2.  **Clone the Repository:**
    ```bash
    git clone https://github.com/adnerscarpelini/ags-todo-api.git
    cd ags-todo-api
    ```

3.  **Restore Dependencies:**
    Navigate to the project directory (the one containing `ags-todo-api.csproj`) and run:
    ```bash
    dotnet restore
    ```

4.  **Configure JWT Key (Important for Authentication):**
    Open `appsettings.json` (and `appsettings.Development.json` if you use it for overrides). Locate the `Jwt:Key` setting.
    ```json
    "Jwt": {
      "Key": "YOUR_SUPER_SECRET_KEY_REPLACE_THIS_AND_KEEP_IT_SAFE",
      "Issuer": "ags-todo-api",
      "Audience": "ags-todo-api-users"
    }
    ```
    Replace `"YOUR_SUPER_SECRET_KEY_REPLACE_THIS_AND_KEEP_IT_SAFE"` with your own strong, unique secret key. The current key is an example and should not be used in a real public scenario.

5.  **Apply Database Migrations:**
    This command will create the `agsTodo.db` SQLite database file and apply the schema based on the defined models and configurations.
    ```bash
    dotnet ef database update
    ```
    *(If you encounter issues with `dotnet ef` commands, ensure you have the EF Core tools installed, either globally: `dotnet tool install --global dotnet-ef` or as a local tool.)*

6.  **Run the Application:**
    ```bash
    dotnet run
    ```
    The API will typically start on `https://localhost:7XXX` and `http://localhost:5XXX` (the exact ports are defined in `Properties/launchSettings.json`). The console output will indicate the URLs.

7.  **Access Swagger UI:**
    Open your browser and navigate to `https://localhost:<https_port>/swagger` (e.g., `https://localhost:7123/swagger`).
    You can use the Swagger UI to explore and test the API endpoints.

## API Endpoints

The API provides the following main functionalities, documented via Swagger:

### Authentication (`/api/auth`)

* **`POST /register`**: Registers a new user.
    * Request Body: `UserRegisterDto` (`username`, `password`)
* **`POST /login`**: Logs in an existing user.
    * Request Body: `UserLoginDto` (`username`, `password`)
    * Response: `LoginResponseDto` (`token`, `username`, `expiration`)
* **`GET /testauth`** (Protected): A simple endpoint to test if authentication is working with a valid token.

### Tasks (`/api/tasks`) - All endpoints are protected

* **`GET /`**: Retrieves all tasks for the authenticated user.
* **`GET /{taskId}`**: Retrieves a specific task by its ID for the authenticated user.
* **`POST /`**: Creates a new task for the authenticated user.
    * Request Body: `TaskCreateDto` (`title`, `description` (optional), `dueDate` (optional))
* **`PUT /{taskId}`**: Updates an existing task for the authenticated user.
    * Request Body: `TaskUpdateDto` (fields are optional)
* **`DELETE /{taskId}`**: Deletes a specific task for theauthenticated user.

---

*This project was created for study and learning purposes to practice API development with C# .NET.*