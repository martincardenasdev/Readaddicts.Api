# ReadaddictsNET 8
Social media for book lovers built with clean architecture and minimal API

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
    "DefaultConnection": "SQL Server database connection string here",
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
    "Issuer": "readaddicts.tech",
    "Audience": "readaddicts.tech"
  }
}
```

Run migrations againts local database
> [!WARNING]
> Must install dotnet-ef tools before running this command

```dotnet ef database update```
