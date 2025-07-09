
# StealTheCats

StealTheCats is an ASP.NET Core Web API application that interacts with The Cat API to fetch and store cat images along with related metadata. It uses Entity Framework Core with SQL Server and Hangfire for background job processing.

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server) (local or remote)
- (Optional) [Visual Studio 2022](https://visualstudio.microsoft.com/) or VS Code
- Internet connection for fetching data from The Cat API

---

## Setup Instructions

### 1. Clone the repository

```bash
git clone https://github.com/Kavalieros/StealTheCats.git
cd StealTheCats
```

### 2. Configure the database connection

Edit `appsettings.json` or your user secrets to set your SQL Server connection strings:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=CatDb;Trusted_Connection=True;TrustServerCertificate=True;",
  "HangfireConnection": "Server=localhost;Database=CatDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

Adjust the `Server`, authentication, and other parameters to match your environment.

---

### 3. Setup Azure Key Vault (optional)

If you use Azure Key Vault for secret management, configure the `CatVaultUri` in your environment or user secrets. Otherwise, store your API key securely in user secrets or environment variables.

eg 

{
  "TheCatApi": {
    "ApiKey": "live_xxxxxx"
  }
}

---

### 4. Apply database migrations

Ensure your SQL Server instance is running.

Then run the EF Core migrations to create/update the database schema:

```bash
dotnet ef database update
```

This will create the `Cats`, `Tags`, and `CatTags` tables along with Hangfire tables.

---

### 5. Build the application

Restore packages and build:

```bash
dotnet restore
dotnet build
```

---

### 6. Run the application

```bash
dotnet run
```

By default, the app will listen on `https://localhost:5051` (check your launch settings).

---

### 7. Access Swagger UI (API documentation)

Navigate to:

```
https://localhost:port/swagger

see launchSettings.json for the correct port
```

Use Swagger UI to explore and test the API endpoints such as fetching cats, checking job status, and retrieving stored cat images.

---

### 8. Hangfire Dashboard

Access Hangfire dashboard at:

```
https://localhost:port/hangfire

see launchSettings.json for the correct port
```

This lets you monitor background jobs like the cat images fetching job.

---

## Usage Tips

- Use the **POST** `/api/cats/fetch` endpoint to start fetching cat images asynchronously.
- Use the **GET** `/api/jobs/{id}` endpoint to check the status of background jobs.
- Use the **GET** `/api/cats` endpoint to retrieve stored cats with pagination and optional tag filtering.
- Use the **GET** `/api/cats/id?id={catId}` to retrieve a specific cat image by ID.

---

## Troubleshooting

- Ensure SQL Server is accessible and connection strings are correct.
- If migrations fail, delete existing DB and try applying migrations fresh.
- Check app logs for detailed error messages.
- (Optional) Enable EF Core sensitive data logging for debugging:

  ```csharp
  optionsBuilder.EnableSensitiveDataLogging();
  ```
  
---

## Unit Tests

This project includes unit tests that cover key functionalities of the `CatService` by retrieving cats from the API.

### Running Unit Tests

- Navigate to the test project folder (usually `StealTheCats.Tests`):

```bash
cd StealTheCats.Tests
```

- Run the tests using the .NET CLI:

```bash
dotnet test
```

This will build the test project, execute all tests, and display results in the console.

### What is tested?

- API calls to The Cat API are mocked to avoid network dependencies.
- Entity Framework Core InMemory database is used to test data operations.
- The API key configuration and mapping dependencies are mocked for isolated testing.

## Contact

For questions or feedback, please contact [Vaggelis Kavalieros](mailto:kavalieros.v@gmail.com).

