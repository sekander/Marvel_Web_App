# Marvel Comics Web App

This repository contains the backend source code for the **Marvel Comics Web App**, a modern role-based ASP.NET Core application designed for managing and selling comic books. The system is architected using a clean, layered approach and features a secure RESTful API, integrated user role management, and seamless e-commerce capabilities.

---

## 🔑 Core Features

* **🔐 Role-Based Access Control (RBAC):**
  Built on ASP.NET Core Identity, the app supports three user roles:

  * **Admin:** Manages users and roles.
  * **Manager:** Maintains comic book inventory.
  * **User:** Browses, purchases comics, and manages a shopping cart.

* **🧩 RESTful API:**
  A robust set of API endpoints enable complete CRUD operations for users, inventory, and purchases—segregated by role.

* **👥 User & Role Management:**
  Admins can perform full user management via the `/api/admin` endpoints, including role assignment and removal.

* **📦 Inventory Management:**
  Managers access `/api/manager` endpoints to create, update, and delete comic records.

* **🛒 E-Commerce Functionality:**
  Users interact through `/api/user` to browse comics, manage carts, and complete purchases.

* **🧪 Database Seeding & API Integration:**
  A Bash script is used to populate the database with Marvel comic book data. It integrates with the **Marvel Developer API**, using secured hash authentication to fetch real comic book data programmatically for preloading characters and comics.

* **🧱 Generic Architecture:**
  Utilizes a `Generic Repository & Service Pattern` with reusable services and controllers for cleaner, DRY-compliant CRUD logic.

---

## 🏛️ Architectural Overview

* **Framework:** ASP.NET Core MVC + Web API
* **Language:** C#
* **ORM:** Entity Framework Core (MySQL via `Pomelo.EntityFrameworkCore.MySql`)
* **Authentication & Authorization:** ASP.NET Core Identity
* **Design Patterns:** Dependency Injection, Generic Services, Clean Separation of Concerns

---

## 📁 API Endpoints Summary

### 🔧 Admin API (`/api/admin`)

* `GET /users`, `GET /users/{id}`
* `POST /users`
* `PUT /users/{id}`
* `DELETE /users/{id}`
* `POST /assign-role/{userId}`

### 📦 Manager API (`/api/manager`)

* `GET /comic/{id}`
* `POST /comics`
* `PUT /comic/{id}`
* `DELETE /comics/{id}`

### 🛍️ User API (`/api/user`)

* `GET /comics`
* `POST /shopping-cart/add/{comicId}`
* `POST /purchase`

---

## 🧩 Key Components

### `Program.cs`

* Configures:

  * **MySQL DB Context**
  * **ASP.NET Core Identity**
  * **Dependency Injection for Generic Services**
  * **Database Initialization and Role Seeding**

### `DbInitializer.cs`

* Seeds:

  * `Admin`, `Manager`, `User` roles
  * Default Admin user
  * Initial data via external Marvel API (via Bash script)

### Controllers

* **`AccountController`**: Login/logout, dashboard redirects by role
* **Role-based MVC Controllers**: Serve dashboards and views
* **Generic `BaseEntityController<T>`**: Handles shared CRUD logic
* **Role-Specific API Controllers**: Implement business logic per role

---

## 🧰 Technologies Used

| Category              | Tool/Library                                         |
| --------------------- | ---------------------------------------------------- |
| Backend Framework     | ASP.NET Core                                         |
| Language              | C#                                                   |
| Authentication        | ASP.NET Core Identity                                |
| Database ORM          | Entity Framework Core                                |
| DB Provider           | Pomelo.EntityFrameworkCore.MySql                     |
| External API          | [Marvel Developer API](https://developer.marvel.com) |
| Scripting (Data Seed) | Bash + `curl` + `jq`                                 |

---

## 🗃️ Marvel API Integration (Data Seeder)

A custom **Bash script** interacts with the **Marvel Developer API** to fetch and populate the database with real Marvel comic book data. This script:

* Uses Marvel API public/private key authentication (timestamp + MD5 hashing).
* Fetches superhero data like Spider-Man, Iron Man, Thor, etc.
* Extracts comic book details and loads them into the database.
* Supports filtering by character, creator, series, issue, and year.
* Limits request count to avoid hitting API rate limits.

*This provides a realistic starting dataset and adds richness to the user experience out of the box.*

---

## 🚀 Getting Started

1. Clone the repository.
2. Set up your database connection in `appsettings.json`.
3. Run the project.
4. Default roles and admin user will be seeded automatically.
5. (Optional) Run the Marvel Bash data seeder to populate the comic book inventory.

---

## 📄 License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---
