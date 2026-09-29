# ElecStore - Full-Stack Management System

A robust N-Tier architecture application built with ASP.NET Core 8 Web API and modern JavaScript, designed for managing electronics store inventory, authentication, and live sales transactions.

---

## 🚀 Key Features

- **N-Tier Architecture:** Clear separation of concerns between API, Business Logic, Data Access Layer, and UI.
- **Authentication & Authorization:** Secure access using JWT (JSON Web Tokens).
- **Product & Category CRUD:** Comprehensive management for electronics inventory.
- **Live Sales & Stock Validation:** Real-time invoice system that verifies stock availability before completing transactions.
- **RTL UI Support:** Responsive and intuitive user interface built with Bootstrap 5 RTL.

---

## 🛠️ Tech Stack

- **Backend:** C#, ASP.NET Core 8 Web API
- **Database Access:** ADO.NET, Microsoft SQL Server
- **Frontend:** JavaScript (ES6+), HTML5, CSS3, Bootstrap 5 RTL
- **Documentation & Testing:** OpenAPI / Scalar / Swagger

---

## 🏗️ Solution Structure

```text
ElecStore-FullStack-API/
│
├── ElecStoreAPI/        # REST API Controllers, DTOs & Middleware
├── ElecStoreBusiness/   # Core Business Logic & Validation Rules
├── ElecStoreDataAccess/ # Database queries & ADO.NET Data Context
├── ElecStoreDB/         # SQL Scripts, Database Schemas & Stored Procedures
└── ElecStoreUI/         # Frontend Web Application (HTML/JS/Bootstrap)
