
# MataPizza.Backend

This project is a dashboard-only backend API built with ASP.NET Core 9 and Entity Framework Core. It provides endpoints to manage and monitor pizza types, orders, and related data for data analysis purposes.




## Features

* **Pizza Type Management** – view all pizza types with detailed ingredients and pricing by size

* **Order Analytics** – view paginated orders with total price, items, and filtering by:

    * **Order ID** (partial match)

    * **Date Range**

    * **Price Range**

* **Search & Filters** – optimized endpoints for dashboard queries

* **Pagination & Aggregates** – paginated results with total item and price summaries

* **DTOs for Clean Responses** – data shaped specifically for dashboard needs


## Tech Stack


* ASP.NET Core 9

* Entity Framework Core

* SQL Server

* RESTful API architecture


## Setup & Run
1. Clone the repository

        git clone https://github.com/rkcbalanza/matapizza-backend.git
        cd matapizza-backend

2. Configure DB in **appsettings.json**

        
        "ConnectionStrings": {
        "DefaultConnection": "Server=localhost;Database=PizzaDashboard;Trusted_Connection=True;"
        }

3. Apply DB Migrations

        dotnet ef database update

4. Run the project

        dotnet run

## Key Endpoints

|Method|Endpoint|Description|
|------|--------|-----------|
|GET|/api/PizzaTypes/paginated|fetch all PizzaTypes paginated|
|GET|/api/PizzaTypes/{id}|fetch PizzaType by id|
|GET|/api/Orders/paginated|fetch all Orders paginated|
|GET|/api/OrderDetails/order/{orderId}|fetch all OrderDetails by OrderId|