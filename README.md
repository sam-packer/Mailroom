# Mailroom Management System

Built with .NET 9.0. Feature complete with user management, package management, MySQL, and JWT authentication.

### Setup Secrets

```bash
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "generate at https://jwtsecret.com/generate"
dotnet user-secrets set "Jwt:Issuer" "your.domain.com"
dotnet user-secrets set "Jwt:Audience" "your.domain.com"
dotnet user-secrets set "ConnectionStrings:MailroomDB" "server=<ip>;user=<username>;password=<password>;database=<database>"
dotnet user-secrets set "Mail:From" "no-reply@mailroom.example.com"
dotnet user-secrets set "Mail:Server" "smtp.example.com"
dotnet user-secrets set "Mail:Port" "587"
dotnet user-secrets set "Mail:Username" "smtpuser"
dotnet user-secrets set "Mail:Password" "smtppassword"
```

### Database Schema

Please look at the [schema.sql](schema.sql) file for the database schema. You can create a new database, switch to it,
and import the schema in.

### Running the Application

```bash
dotnet restore
dotnet run
```