# 📦 Bulky Web Application

**Bulky** is a complete e-commerce web application built with **ASP.NET Core MVC**, **Entity Framework Core**, and **SQL Server**. The system includes user and role management, product and order management, and Stripe payment integration. It follows a clean, modular **N-Tier Architecture** and applies design patterns like **Repository**, **Unit of Work**, and uses **Redis** for cart management and caching.

---

## 🧱 N-Tier Architecture

This project is structured using **N-Tier Architecture**, ensuring clear separation of concerns:

### 1. Presentation Layer – `BulkyWeb`
- ASP.NET Core MVC frontend
- Razor Views, Controllers, ViewModels
- Areas for modular UI separation: `Admin`, `Customer`, `Identity`
- Custom Middleware for exception handling

### 2. Business Logic Layer (BLL) – `Bulky.BL`
- Business services and logic
- AutoMapper profiles
- Shared DTOs
- Services for each entity and ServiceManager

### 3. Data Access Layer (DAL) – `Bulky.DataAccess`
- Entity Framework Core entities and configuration
- Generic and specific repositories
- Unit of Work pattern for transaction consistency
- Database initializers and migrations

### 4. Utility Layer – `Bulky.Utility`
- Shared components across layers like:
  - `EmailSender.cs` for sending emails
  - `StripeSetting.cs` for payment configuration

---

## ✅ What I Did

Here’s a summary of my contributions:

- ✅ Set up and configured the **database** using EF Core with code-first migrations.
- ✅ Applied **Repository** and **Unit of Work** patterns to manage data operations cleanly.
- ✅ Built both **Admin** and **Customer** areas with features like product, category, user, and company management.
- ✅ Implemented **Stripe** payment integration for secure checkout and order placement.
- ✅ Integrated **Identity and Role Management** using ASP.NET Core Identity.
- ✅ Used **AutoMapper** with custom profiles for smooth data transformation between ViewModels and Entities.
- ✅ Integrated **Redis** to handle and cache the **shopping cart**, improving performance and scalability.
- ✅ Developed custom middleware and centralized exception handling.
- ✅ Followed **clean architecture principles** for separation of concerns and maintainability.

---

## 💻 Technologies Used

- ASP.NET Core MVC  
- Entity Framework Core  
- SQL Server  
- ASP.NET Identity  
- AutoMapper  
- Redis (StackExchange.Redis)  
- Stripe API  
- Bootstrap 5 + jQuery  
- Razor Views  
- Clean Architecture (N-Tier)

---
