# Readaddicts.Api
Social media for book lovers built with clean architecture and minimal APIs, using .NET 8 and SQL Server

> [!IMPORTANT]
> To run this project, you need .NET 8 SDK and Docker installed in your system

* Pull SQL Server image:

```docker pull mcr.microsoft.com/mssql/server:2022-latest```

* Run a docker container named sqlserver2022 and expose to port 1433 (SQL Server default port):

```
docker run --name sqlserver2022 -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=yourStrong(!)Password" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
```

Test connection with DBeaver or other database management tools<sup>Optional</sup>

```
username: sa
password: yourStrong(!)Password
```

Configure appsettings.json
```
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=master;User Id=sa;Password=yourStrong(!)Password;",
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Jwt": {
    "Secret": "JWT Secret here",
    "Issuer": "readAddicts.tech",
    "Audience": "readAddicts.tech"
  }
}
```

If you get problems with the certificates when trying to run the app, you can add ```TrustServerCertificate=true;``` to the connection string to skip SSL certificate validation

> [!WARNING]
> Bypassing SSL certificate validation is never recommended unless you're running development/local environments

## Migrations
> [!IMPORTANT]
> Must install dotnet-ef tools before running these commands

List all the migrations:
```dotnet ef migrations list --project Infrastructure --startup-project Readaddicts.Api```

Define a new migration:
```
dotnet ef migrations add {migration_name} --project Infrastructure  --startup-project Readaddicts.Api --output-dir ../Infrastructure/Data/Migrations
```

Run migrations against local database:
```dotnet ef database update --project Infrastructure  --startup-project Readaddicts.Api```
