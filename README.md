# ASP.NET Core with Syncfusion demo | C# - ASP.NET Core - Razor Pages - Syncfusion

## Author

**Kevin DOOLAEGHE**

## References

* [Microsoft Learn - Razor Pages with Entity Framework Core in ASP.NET Core](https://learn.microsoft.com/fr-fr/aspnet/core/data/ef-rp/intro)
* [Syncfusion Website](https://www.syncfusion.com/)

## Microsoft SQL Server

* Connect to the SQL Server instance :
```
/opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "Password.123"
```

* Show all databases :
```
SELECT Name FROM sys.databases;
GO
```

* Show all tables in `Demo` database :
```
USE Demo;
GO
SELECT Name FROM sysobjects WHERE xtype='U';
GO
```

* Describe the `Records` table :
```
SELECT c.name FROM sys.columns AS c
INNER JOIN sys.tables AS t ON t.object_id = c.object_id
INNER JOIN sys.schemas AS s ON s.schema_id = t.schema_id
WHERE t.name = 'Records' AND s.name = 'dbo';
GO
```