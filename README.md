# Garage Operations Monolith

A robust, server-rendered web application built with ASP.NET Core MVC for managing automotive service centers. This system handles customer data, vehicle servicing tracking, and operational reporting, utilizing a strict separation of concerns through DTOs and optimized data-fetching mechanisms.

## 🏗 System Architecture

The application follows a monolithic MVC (Model-View-Controller) architecture, heavily emphasizing performance and memory management at the data access layer.

* **View Layer:** Server-side rendered views utilizing Razor pages. Dynamic data injection handles state management and user interface rendering.
* **Controller Layer:** Acts as the orchestrator. Validates incoming HTTP requests, maps domain entities to DTOs, and executes business logic securely.
* **Model/Data Layer:** Bypasses traditional Entity Framework Core LINQ abstractions in favor of direct `ExecuteSqlRawAsync` and raw SQL queries. This architectural constraint enforces explicit control over execution plans and query optimization.

## ✨ Core Technical Features

* **Optimized Server-Side Pagination:** The system actively prevents memory overflows and slow load times. Large datasets (e.g., active vehicle services, customer lists) are never fetched entirely. Pagination is handled at the database level using `OFFSET` and `FETCH`, ensuring only the requested subset of rows (e.g., 10 records per page) is retrieved and transmitted to the client per request.
* **Strict Data Transfer Objects (DTOs):** Absolute separation between database entities and view representations. Controllers strictly handle incoming requests and outgoing responses via validated DTOs, mitigating mass-assignment vulnerabilities.
* **Advanced Data Filtering & Reporting:** Built-in reporting interfaces utilizing parameterized SQL queries to filter service records by custom parameters (e.g., date ranges, service status).
* **Strict Entity Validation:** Implements comprehensive domain validation, including rigorous international standard checks for 17-character VINs (Vehicle Identification Numbers), prohibiting illegal characters (I, O, Q) and ensuring structural integrity before database commits.
* **Resilient Exception Handling:** Granular `try-catch` implementations at the controller level, specifically handling `DbUpdateConcurrencyException` to manage database lock conflicts and state inconsistencies smoothly.

## 🛠 Tech Stack

* **Backend:** C#, ASP.NET Core MVC (.NET 8.0)
* **ORM / Data Access:** Entity Framework Core (Strictly utilized for DB context management and Raw SQL execution)
* **Database:** Microsoft SQL Server
* **Frontend:** HTML5, CSS, JavaScript (Razor Views)
* **Tooling:** SQL Server Management Studio (SSMS)

## 🚀 Getting Started

### Prerequisites
* .NET 8.0 SDK or later.
* Microsoft SQL Server and SSMS installed locally.

### Installation & Setup

1.  **Clone the repository:**
    ```bash
    git clone [https://github.com/YourUsername/garage-operations-monolith.git](https://github.com/YourUsername/garage-operations-monolith.git)
    cd garage-operations-monolith
    ```

2.  **Database Provisioning:**
    * Open SQL Server Management Studio (SSMS).
    * Locate the provided SQL script file (`database_setup.sql`) in the repository.
    * Execute the script. This will automatically generate the `Garage_Management` database, provision the necessary tables, configure primary/foreign keys, and insert the initial seeding data.

3.  **Configure Connection:**
    * Open `appsettings.json`.
    * Verify the `DefaultConnection` string points to your local SQL Server instance.

4.  **Run the Application:**
    * Build and run the project via Visual Studio or the .NET CLI:
    ```bash
    dotnet run
    ```
    * Navigate to `http://localhost:<port>` to access the system. The landing page acts as the entry point (`SignUp`/`Login`).

## 🛡️ Security & Constraints Note
Due to the strict project requirement of utilizing `SQL RAW` over standard EF core abstractions, significant emphasis has been placed on validating DTO inputs using `asp-validation-for` and explicit type casting to mitigate standard injection vectors, ensuring operational stability.





https://docs.google.com/document/d/1l7b0ap2CR5FekAoydLbPzA_q6qbFNIIWDVv9u20WAoc/edit?usp=sharing
