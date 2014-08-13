ServiceStack.OrmLite.Additions
========================
This package adds useful functionality to OrmLite v3 

## ReliableSqlServerOrmLiteDialectProvider

This is an implementation of OrmLite's SqlServer Dialect Provider that utilizes transient fault handling.  The connections created are instances of ReliableSqlConnection from the Enterprise Library's Transient Fault Handling Application Block.

To use, simply replace the regular SqlServer dialect provider with the reliable version when creating the OrmLiteConnectionFactory:

```csharp
var factory = new OrmLiteConnectionFactory(connectionString, ReliableSqlServerOrmLiteDialectProvider.Instance);
container.Register<IDbConnectionFactory>(db);
```

## OrmLiteConfigurator
