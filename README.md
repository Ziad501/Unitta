# Unitta - Luxury Unit Booking Platform

A professional-grade hotel booking platform built with Clean Architecture principles, featuring real-time availability checking, secure payment processing, and comprehensive administrative tools for luxury accommodation management.

**Live Demo:** ([Unitta](https://unitta.runasp.net/))

## Core Features

**Guest Experience**
- Real-time unit availability with dynamic date filtering
- Secure payment processing through Stripe integration
- User registration and authentication system
- Automated email confirmations via SendGrid
- Responsive mobile-friendly interface

**Administrative Dashboard**
- Interactive analytics with ApexCharts visualization
- Complete booking lifecycle management (check-in/checkout/cancellation)
- CRUD operations for units, features, and room assignments
- Document generation for booking reports using OpenXML
- Role-based access control with administrative privileges

**Technical Capabilities**
- Asynchronous booking conflict detection
- Image upload and validation for unit galleries
- AJAX-powered user interactions without page reloads
- Comprehensive data validation with FluentValidation
- Excel export functionality for booking data

## Architecture & Design

**Clean Architecture Implementation**
- **Domain Layer** - Core business entities isolated from external dependencies
- **Application Layer** - Use cases, interfaces, and business logic orchestration
- **Infrastructure Layer** - Database access, external API integrations, identity management
- **Presentation Layer** - ASP.NET Core MVC with responsive UI components

**Design Principles Applied**
- SOLID principles throughout the codebase
- Repository pattern for data access abstraction
- Dependency inversion with comprehensive interface usage
- Single responsibility principle in service design
- Adapter pattern for identity service abstraction

## Technology Stack

| Layer | Technologies |
|-------|-------------|
| **Backend** | ASP.NET Core 8, Entity Framework Core, C# 12 |
| **Database** | SQL Server with Fluent API configuration |
| **Authentication** | ASP.NET Core Identity with role-based authorization |
| **Validation** | FluentValidation with asynchronous database validation |
| **Mapping** | Riok.Mapperly (compile-time source generation) |
| **Payments** | Stripe API with webhook verification |
| **Email** | SendGrid API integration |
| **Frontend** | Bootstrap 5, JavaScript ES6, AJAX, ApexCharts |
| **Document Processing** | OpenXML SDK for Excel generation |

## Key Implementation Highlights

**Performance Optimizations**
- Compile-time mapping with zero runtime reflection overhead
- Strategic database indexing for booking conflict queries
- Efficient EF Core query patterns with proper entity tracking
- Asynchronous operations throughout the application stack

**Security Measures**
- Server-side payment verification with Stripe webhooks
- Role-based authorization with custom middleware
- Image validation using ImageSharp content verification
- SQL injection prevention through parameterized queries

**Code Quality Standards**
- Comprehensive validation at multiple architectural layers
- Consistent error handling and logging patterns
- Separation of concerns with dedicated service layers
- Unit testable architecture through dependency injection

## Business Logic Highlights

The application demonstrates sophisticated business rule implementation including booking conflict detection, dynamic pricing calculations, automated status management, and complex reporting capabilities. The architecture supports future scalability with clearly defined boundaries between business logic and infrastructure concerns.

MIT License
