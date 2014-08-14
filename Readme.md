ServiceStack.OrmLite.Additions [![Build status](https://ci.appveyor.com/api/projects/status/6mqys3gd83ka9ai9)](https://ci.appveyor.com/project/jarroda/servicestack-ormlite-additions)
========================
This package adds useful functionality to OrmLite v3 

## Installation

Install the NuGet package
```
PM> Install-Package ServiceStack.OrmLite.Additions
```

## ReliableSqlServerOrmLiteDialectProvider

This is an implementation of OrmLite's SqlServer Dialect Provider that utilizes transient fault handling.  The connections created are instances of ReliableSqlConnection from the Enterprise Library's Transient Fault Handling Application Block.

To use, simply replace the regular SqlServer dialect provider with the reliable version when creating the OrmLiteConnectionFactory:

```csharp
var factory = new OrmLiteConnectionFactory(connectionString, ReliableSqlServerOrmLiteDialectProvider.Instance);
container.Register<IDbConnectionFactory>(db);
```

## OrmLiteConfigurator

The OrmLiteConfigurator is a static class that exposes OrmLite's internal Type -> ModelDefinition mapping.  This allows insepction and modification of the model definition. Additionally, the OrmLiteConfigurator allows the configuration of OrmLite without using attributes on your domain models.  This allows for model classes to reside in an assembly that does not, or can not have a reference to ServiceStack.OrmLite;

To access the mapping or get the ModelDefinition of a type:

```csharp
var map = OrmLiteConfigurator.TypeModelDefinitionMap;
var definition = OrmLiteConfigurator.GetModelDefinition<MyModel>();
```

To configure a type in OrmLite, a fluent interface is provided. Most of OrmLite's features are exposed through the parameters to AddField, including foreign keys.  This should only be done during application startup.

```csharp
OrmLiteConfigurator.TypeModelDefinitionMap.AddModelDefinition<Project>("Projects")
    .AddField(p => p.Id, isPrimaryKey: true, isAutoIncrement: true)
    .AddField(p => p.ProjectName)
    .AddField(p => p.ProjectNumber, defaultValue: "100")
    .AddField(p => p.ProjectManagerId, references: typeof(User), onDelete: "CASCADE", onUpdate: "CASCADE")
    .AddIgnoredField(p => p.ProjectManagerEmail);
```
