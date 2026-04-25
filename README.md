Qash.API

A production-style backend API for Qash, a university mobile expense tracker application.

Built with:

ASP.NET Core Web API (.NET 8)
PostgreSQL
Entity Framework Core
Clean Architecture (inside single project)
CQRS + MediatR
FluentValidation
JWT Authentication
Refresh Tokens
Swagger / OpenAPI
AutoMapper
Project Overview

Qash is a personal finance mobile application that helps users:

Track expenses and income
Manage wallets/accounts
Create budgets
Set saving goals
Analyze reports
Use recurring transactions
Securely manage their profile

This repository contains the backend REST API.

Current Implemented Modules
Authentication

✅ Register
✅ Login with phone number
✅ JWT Access Token
✅ Refresh Token
✅ Logout
✅ Change Password
✅ Forgot Password (OTP Demo)
✅ Phone Verification Demo OTP

Profile

✅ Get Profile
✅ Update Profile
✅ Soft Delete Profile

Tech Stack
Technology	Version
.NET	8
ASP.NET Core Web API	8
EF Core	8.0.11
PostgreSQL	16+
Npgsql	8.0.11
MediatR	12.4.1
FluentValidation	11.11.0
Swagger	6.9.0
AutoMapper	12.0.1
Architecture
Qash.API/
├── Controllers/
├── Domain/
│   ├── Common/
│   ├── Entities/
│   └── Enums/
├── Infrastructure/
│   ├── Data/
│   ├── Authentication/
│   ├── Mapping/
│   └── Services/
├── Features/
│   ├── Auth/
│   └── Profile/
├── Common/
│   ├── Responses/
│   ├── Exceptions/
│   └── Behaviors/
└── Program.cs
Design Principles
Thin Controllers
Business Logic inside Handlers
Validation with FluentValidation
Feature-based folders
Reusable Response Wrapper
Secure Token Authentication
Soft Delete Pattern
Clean Separation of Concerns
Installation Guide
1. Clone Repository
git clone YOUR_REPO_URL
cd Qash.API
2. Install .NET 8 SDK

Download:

https://dotnet.microsoft.com/download/dotnet/8.0

Check:

dotnet --version
3. Install PostgreSQL

Download:

https://www.postgresql.org/download/

Recommended:

PostgreSQL 16+
Port: 5432
Username: postgres
Password: yourpassword
4. Update Connection String

Edit:

appsettings.json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=QashDb;Username=postgres;Password=yourpassword"
}
5. Restore Packages
dotnet restore
6. Create Database
dotnet ef database update
7. Run Project
dotnet run

Swagger:

http://localhost:xxxx/swagger
Authentication System
Access Token
JWT
Short-lived
Refresh Token
Stored in database
Revoked on logout
Rotated on refresh
Demo OTP System

For academic/demo use only:

Verification Code = 00000

Used for:

Register phone verification
Forgot password
Change password

In production this should be replaced by SMS OTP provider.

API Response Format
Success
{
  "success": true,
  "message": "Operation completed successfully",
  "data": {}
}
Failure
{
  "success": false,
  "message": "Operation failed",
  "errors": []
}
API Documentation
Base URL
/api
Auth APIs
Register
POST /api/auth/register
{
  "firstName": "Mohamed",
  "lastName": "Ahmed",
  "email": "mohamed@gmail.com",
  "phoneNumber": "01028239305",
  "verificationCode": "00000",
  "password": "Password123",
  "confirmPassword": "Password123"
}
Login
POST /api/auth/login
{
  "phoneNumber": "01028239305",
  "password": "Password123"
}
Refresh Token
POST /api/auth/refresh-token
{
  "refreshToken": "TOKEN_HERE"
}
Logout
POST /api/auth/logout
{
  "refreshToken": "TOKEN_HERE"
}
Request Forgot Password Code
POST /api/auth/forgot-password/request-code
{
  "phoneNumber": "01028239305"
}
Reset Password
POST /api/auth/forgot-password/reset
{
  "phoneNumber": "01028239305",
  "verificationCode": "00000",
  "newPassword": "Password123",
  "confirmPassword": "Password123"
}
Change Password
POST /api/auth/change-password

Requires JWT token

{
  "oldPassword": "Password123",
  "verificationCode": "00000",
  "newPassword": "NewPassword123",
  "confirmPassword": "NewPassword123"
}
Profile APIs

Requires JWT token

Get Profile
GET /api/profile
Update Profile
PUT /api/profile
{
  "firstName": "Mohamed",
  "lastName": "Ali",
  "email": "new@gmail.com",
  "phoneNumber": "01000000000"
}
Delete Profile
DELETE /api/profile

Soft delete only.

Database Tables
Users
Id
FirstName
LastName
Email
PhoneNumber
PasswordHash
CreatedAt
UpdatedAt
IsDeleted
DeletedAt
RefreshTokens
Id
Token
ExpiresAt
IsRevoked
RevokedAt
UserId
Security Features
Password hashing
JWT authentication
Refresh token rotation
Token revocation
Soft delete accounts
Protected endpoints
Next Planned Modules
Categories
Wallets
Transactions
Budgets
Saving Goals
Reports
Recurring Transactions
Notifications
Dashboard Analytics
Author

University Graduation Project Backend

Built with ASP.NET Core by Mohamed Ahmed

License

For educational use.
