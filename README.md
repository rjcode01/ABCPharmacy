# ABC Pharmacy - Coding Assessment

## Overview

ABC Pharmacy is a Single Page Application developed to manage medicines and maintain medicine sale records.

The application provides functionality to view and search medicines, add new medicines, and record medicine sales while maintaining the available stock.

## Technology Stack

* ASP.NET Core 8 Web API
* HTML5
* CSS3
* JavaScript
* Fetch API
* JSON files for server side data persistence
* Swagger/OpenAPI for API documentation and testing

## Features

### Medicine Management

* Display all available medicines in a grid.
* Add new medicine details.
* Search medicines by name.
* Store medicine data in JSON files on the server.
* Validate medicine details before saving.

### Medicine Information

Each medicine contains:

* Full Name
* Notes
* Expiry Date
* Quantity
* Price
* Brand

The Notes field is not displayed in the medicine grid.

### Stock and Expiry Indicators

The medicine grid provides visual indicators:

* **Red background:** Medicine expiry date is within the next 30 days.
* **Yellow background:** Medicine quantity is less than 10.

### Sales Management

* Record medicine sales.
* Automatically reduce the available stock after a successful sale.
* Prevent sales when the requested quantity exceeds available stock.
* Prevent sales of expired medicines.
* View sales history.

## Running the Application

### Prerequisites

* .NET 8 SDK

### Start the Application

Run the following commands from the project directory:

```bash
dotnet restore
dotnet run
```

After starting the application, open the URL displayed in the terminal.

Swagger API documentation is available at:

```text
/swagger
```

## API Endpoints

### Medicines

```text
GET    /api/medicines
GET    /api/medicines/{id}
POST   /api/medicines
```

Search medicines by name:

```text
GET /api/medicines?search=para
```

### Sales

```text
GET  /api/sales
POST /api/sales
```

## Data Storage

Medicine and sales information is persisted using JSON files on the server side, as required by the assessment.

For a production application, a database such as SQL Server or PostgreSQL would be more appropriate for scalability, concurrency, and reliable data management.

## Validation and Business Rules

The application validates medicine information before saving.

For sales:

* The medicine must exist.
* The medicine must not be expired.
* The requested sale quantity must be greater than zero.
* The requested quantity must not exceed the available stock.
* Stock is reduced after a successful sale.

## Future Improvements

For a production-ready application, the following could be added:

* Database persistence
* Authentication and authorization
* Role-based access
* Unit and integration tests
* Centralized logging
* Global error handling and monitoring
* Pagination for large medicine and sales datasets

## Assessment Scope

This implementation focuses on the functional and technical requirements provided in the ABC Pharmacy coding assessment.
