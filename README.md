# Mailroom Management System

Built with .NET 9.0. Feature complete with user management, package management, PostgresSQL, and authentication.

## Live demo
If you don't want to set up the project, you can view a live demo of this project at
[https://mailroom.sampacker.com](https://mailroom.sampacker.com)

### Admin Credentials
**Username:** `admin@mailroom.com`

**Password:** `admindemo`

### User Credentials
**Username:** `user@mailroom.com`

**Password:** `userdemo`


## Setup Secrets

```bash
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "generate at https://jwtsecret.com/generate"
dotnet user-secrets set "Jwt:Issuer" "your.domain.com"
dotnet user-secrets set "Jwt:Audience" "your.domain.com"
dotnet user-secrets set "ConnectionStrings:MailroomDB" "Host=<ip>;Port=5432;Username=<username>;Password=<password>;Database=<database>"
dotnet user-secrets set "Mail:From" "no-reply@mailroom.example.com"
dotnet user-secrets set "Mail:Server" "smtp.example.com"
dotnet user-secrets set "Mail:Port" "587"
dotnet user-secrets set "Mail:Username" "smtpuser"
dotnet user-secrets set "Mail:Password" "smtppassword"
```

## Database Schema

This project uses EF Core migrations to set up the database. You can run the following commands to set up your own
database with the schema. Please make sure that you run the commands above to set the user secrets. Otherwise, it won't
be able to establish a connection to your database.
```bash
dotnet tool install --global dotnet-ef
dotnet ef database update
```

## Running the Application

```bash
dotnet restore
dotnet run
```