# TrainComponent API

TrainComponent API is a RESTful service designed to manage train components, with support for filtering, searching, pagination, and quantity assignment where applicable.

## Features

- Retrieve a list of components with filtering, sorting, pagination, and search
- Optional full-text search support (based on configuration)
- Create, update, and delete components
- Assign quantities to components (only if allowed)
- Robust validation and error handling
- Swagger UI for interactive API documentation

## Requirements

- .NET 9 SDK or later
- Microsoft SQL Server
- Visual Studio, Rider, or any compatible IDE

## Getting Started

1. Clone the repository.

2. Update the `appsettings.json` configuration:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=TrainComponentsDb;Trusted_Connection=True;TrustServerCertificate=True;"
     },
     "EnableFullTextSearch": false
   }
   ```

3. Run the application:
   ```bash
   dotnet run
   ```

4. Open Swagger UI in your browser.


## Project Structure

```
TrainComponent/
├── Application/
│   └── DTOs/               # Data transfer objects
│   └── Mappers/            # Manual mappers between DTOs and entities
├── Controllers/            # API endpoints
├── Domain/
│   └── Entities/           # EF Core entities
├── Infrastructure/
│   └── ErrorHandling/      # Global error handler
│   └── Persistence/        # Database context and migrations
└── Program.cs              # Application startup
```

## API Overview

### GET /api/components

Query parameters:

| Parameter            | Description                                             |
|----------------------|---------------------------------------------------------|
| `query`              | Search term (matches `Name` and `UniqueNumber`)         |
| `canAssignQuantity`  | Filter by quantity assignment capability (`true/false`) |
| `page`               | Page number (default: 1)                                |
| `pageSize`           | Page size (default: 20)                                 |
| `sortOptions.sortBy` | Sorting field: `name`, `uniquenumber`                   |
| `sortOptions.sortDir`| Sorting direction: `asc`, `desc` (default: `asc`)       |

### POST /api/components
Creates a new component.

### PUT /api/components/{id}
Updates an existing component.

### POST /api/components/{id}/assign-quantity
Assigns a quantity to a component (if allowed).

### DELETE /api/components/{id}
Deletes a component.

## Configuration

- **EnableFullTextSearch**: If `true`, the API uses SQL Server Full-Text Search for `query`. Otherwise, it performs standard `LIKE` searches.

## License

This project is licensed under the MIT License.
