# LifeSync

## Table of Contents

- [Project Overview](#project-overview)
- [Features](#features)
- [Technologies Used](#technologies-used)
- [Setup and Installation](#setup-and-installation)
- [Running the Application](#running-the-application)
  - [With Docker](#with-docker)
  - [Locally](#locally)
- [Quick Start with LifeSync StartupScripts](#quick-start-with-lifesync-startupscripts)

## Project Overview

LifeSync aims to provide a unified platform for managing personal tasks on a daily basis. The application is divided into three main projects:

1. **LifeSync.API**: The backend API developed using .NET, responsible for handling business logic and data management.
2. **LifeSync.Client**: The frontend application built with React and TypeScript, offering an intuitive user interface for interacting with the API.
3. **LifeSync.Tests.Unit**: Unit tests written in .NET xUnit to ensure code reliability and maintainability.

## Features

- **Personal Finance Management**: Track income and expenses.
- **User Profile**: Track and manage personal balance and other info.
- **User Authentication**: Secure login and user management.

## Technologies Used

### Backend
- .NET 10
- ASP.NET Core
- Entity Framework Core

### Frontend
- React
- TypeScript

### Testing
- xUnit

### Database
- SQL Server

## Setup and Installation

Follow these steps to set up your development environment for LifeSync.

### 1. Install and Configure MSSQL Developer Edition

1. **Download & Install:**
   - Download [MSSQL Developer Edition](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) and run the installer. Follow the prompts in the installation wizard.

2. **Configure the Server:**
   - Launch SQL Server Management Studio (SSMS) and connect to your server instance (typically `localhost` on port `1433`).
   - *(Optional)* Adjust the authentication mode (Windows or Mixed Mode) as needed.

3. **Verify the Connection:**
   - Ensure the server is running and you can connect to it.

---

### 2. Configure Local Secrets for ASP.NET Core

To securely store sensitive information (e.g., JWT secret, database credentials), use ASP.NET Core’s user secrets.

1. **Initialize User Secrets:**
   - Open a terminal in your ASP.NET Core project folder (the folder containing your `.csproj` file) and run:
     ```bash
     dotnet user-secrets init
     ```

2. **Add Your Local Secrets:**
   - Create or update your local secrets with the following JSON (replace placeholder values with your actual credentials):
     ```json
     {
       "AppSecrets": {
         "JWT": {
           "SecretKey": "INSERT_YOUR_SECRET",
           "Issuer": "LifeSyncApp",
           "Audience": "LifeSyncAppUsers",
           "ExpiryMinutes": 60
         },
         "Database": {
           "Username": "INSERT_YOUR_USERNAME",
           "Password": "INSERT_YOUR_PASSWORD",
           "Engine": "",
           "Host": "localhost",
           "Port": 1433,
           "DbInstanceIdentifier": "LifeSync"
         }
       }
     }
     ```
   - Alternatively, set individual secrets via the command line:
     ```bash
     dotnet user-secrets set "AppSecrets:JWT:SecretKey" "YOUR_SECRET_KEY"
     dotnet user-secrets set "AppSecrets:Database:Username" "YOUR_DB_USERNAME"
     dotnet user-secrets set "AppSecrets:Database:Password" "YOUR_DB_PASSWORD"
     ```
   **Important:** User secrets are stored locally (typically under your user profile) and are not committed to source control.

   For more details, refer to [Microsoft's documentation on Secret Manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets).

---

### 3. Apply Entity Framework Core Migrations

1. **Update the Connection String:**
   - Ensure your ASP.NET Core configuration (in *appsettings.json* or via user secrets) correctly points to your `LifeSync` MSSQL database.

2. **Run Migrations:**
   - Open a terminal in your project’s solution directory and run:
     ```bash
     dotnet ef database update
     ```
   - This command applies all pending EF Core migrations to the `LifeSync` database.

   For further information, see the [EF Core Migrations documentation](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/).

---

## Running the Application

You can run the whole stack with Docker (no local MSSQL or secrets setup required), or run each project manually.

### With Docker

This is the fastest way to get everything running. It builds and starts the API, both frontends, and SQL Server in containers.

**Prerequisites:** [Docker Desktop](https://www.docker.com/products/docker-desktop/) (or Docker Engine + Compose) installed and running.

**Steps:**

1. From the repository root, start all services:
   ```bash
   docker compose up -d --wait
   ```
   The first run builds the images and applies the database setup; subsequent runs reuse them.

2. Once the containers are healthy, access the apps:
   - API: `http://localhost:7200`
   - React frontend: `http://localhost:4200`
   - Angular frontend: `http://localhost:4300`
   - SQL Server: `localhost:1433`

3. To stop the stack (keeps the database volume):
   ```bash
   docker compose down
   ```

> Configuration is read from the `.env` file in the repository root. No additional setup is needed for local development.

### Locally

#### 1. Run the Backend API

1. **Start the API:**
   - Navigate to your ASP.NET Core backend project folder.
   - Run:
     ```bash
     dotnet run
     ```
   - Your backend server will start (by default, it listens on a port such as `http://localhost:5000` or as configured in your project).

2. **Verify the API:**
   - Open your browser and navigate to the API’s URL (e.g., `http://localhost:5000`) to confirm that it is running properly.

---

#### 2. Run the React Frontend

1. **Install Dependencies:**
   - Navigate to your React project folder.
   - Run:
     ```bash
     npm install
     ```

2. **Start the Frontend:**
   - Once the dependencies are installed, run:
     ```bash
     npm start
     ```
   - The frontend should start automatically (typically on port 3000 or 4200). If it doesn’t open automatically, navigate to the URL provided in the terminal.

---

## Quick Start with LifeSync StartupScripts

Instead of running the backend API and React frontend manually, you can simply use the provided LifeSync shortcut located in the `LifeSync.StartupScripts` folder. This shortcut automates the process of launching both the backend and frontend with one click.
