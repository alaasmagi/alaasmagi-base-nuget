# alaasmagi Base NuGet Packages

Reusable base-layer packages for .NET applications built around a layered architecture.

This repository contains the source for the `alaasmagi.Base.*` package set. The packages provide shared contracts and base implementations for:

- domain entities
- DTO and mapper abstractions
- method-response and error models
- repositories and unit of work
- EF Core data access
- application services
- typed exceptions

## Packages

The repository currently contains these NuGet packages:

- `alaasmagi.Base.Contracts.Domain`
- `alaasmagi.Base.Domain`
- `alaasmagi.Base.Contracts.DTO`
- `alaasmagi.Base.DTO`
- `alaasmagi.Base.Contracts.DataAccess`
- `alaasmagi.Base.DataAccess.EF`
- `alaasmagi.Base.Contracts.Application`
- `alaasmagi.Base.Application`
- `alaasmagi.Base.Contracts.Exception`
- `alaasmagi.Base.Exception`

All projects currently target `.NET 10.0`.

## What Each Package Does

### Domain

- `Base.Contracts.Domain` defines entity contracts such as `IBaseEntity<TKey>`, `IBaseEntityMeta`, `IBaseEntitySoftDelete`, and `IBaseEntityUserId<TKey>`.
- `Base.Domain` provides base classes such as `BaseEntity<TKey>`, `BaseEntityWithMeta<TKey>`, `BaseEntityWithMetaSoftDelete<TKey>`, `BaseEntityUserWithMeta<TKey, TUserKey>`, and `BaseEntityUserWithMetaSoftDelete<TKey, TUserKey>`.

### DTO

- `Base.Contracts.DTO` defines `IMapper<...>`, `IMethodResponse<...>`, and `IError<...>`.
- `Base.DTO` provides `MethodResponse`, `Error`, and shared `ErrorDefaults`.

### Data Access

- `Base.Contracts.DataAccess` defines repository contracts and `IBaseUow`.
- `Base.DataAccess.EF` provides generic EF Core implementations for repository and unit-of-work patterns, including soft delete support.

### Application

- `Base.Contracts.Application` defines generic CRUD and soft-delete service contracts.
- `Base.Application` provides generic application service implementations that map between application models and repository/domain models.

### Exceptions

- `Base.Contracts.Exception` defines `IBaseException` and `IHttpException`.
- `Base.Exception` provides `BaseException` and `HttpException`.

## Typical Flow

The package set is designed for a layered setup:

1. Define a domain model by inheriting from one of the `Base.Domain` base classes.
2. Define a persistence model if your EF entity differs from the domain model.
3. Implement an `IMapper<TUpperEntity, TLowerEntity, TKey>` for the repository layer.
4. Inherit from `BaseRepository<...>` or `BaseRepositorySoftDelete<...>`.
5. If your application layer exposes different models, implement another mapper between application models and domain models.
6. Inherit from `BaseService<...>` or `BaseServiceSoftDelete<...>`.

## Example

```csharp
using Base.Contracts.DTO;
using Base.DataAccess.EF;
using Base.Domain;
using Microsoft.EntityFrameworkCore;

public class Todo : BaseEntityWithMetaSoftDelete<Guid>
{
    public string Title { get; set; } = default!;
}

public class TodoDbEntity : BaseEntityWithMetaSoftDelete<Guid>
{
    public string Title { get; set; } = default!;
}

public class TodoMapper : IMapper<Todo, TodoDbEntity, Guid>
{
    public Todo? Map(TodoDbEntity? entity) => entity == null
        ? null
        : new Todo
        {
            Id = entity.Id,
            Title = entity.Title,
            CreatedAt = entity.CreatedAt,
            CreatedBy = entity.CreatedBy,
            UpdatedAt = entity.UpdatedAt,
            UpdatedBy = entity.UpdatedBy,
            IsDeleted = entity.IsDeleted
        };

    public IEnumerable<Todo>? Map(IEnumerable<TodoDbEntity>? entities) =>
        entities?.Select(Map)!;

    public TodoDbEntity? Map(Todo? entity) => entity == null
        ? null
        : new TodoDbEntity
        {
            Id = entity.Id,
            Title = entity.Title,
            CreatedAt = entity.CreatedAt,
            CreatedBy = entity.CreatedBy,
            UpdatedAt = entity.UpdatedAt,
            UpdatedBy = entity.UpdatedBy,
            IsDeleted = entity.IsDeleted
        };

    public IEnumerable<TodoDbEntity>? Map(IEnumerable<Todo>? entities) =>
        entities?.Select(Map)!;
}

public class TodoRepository
    : BaseRepositorySoftDelete<Todo, TodoDbEntity, TodoMapper, Guid, Guid>
{
    public TodoRepository(DbContext dbContext, TodoMapper mapper)
        : base(dbContext, mapper)
    {
    }
}
```

## Notes

- Package versions should be kept aligned across the full `alaasmagi.Base.*` set.
- `Base.DataAccess.EF` expects Entity Framework Core.
- Metadata fields such as `CreatedAt`, `UpdatedAt`, `CreatedBy`, and `UpdatedBy` are managed by the EF repository base classes when the entity supports metadata.
- User scoping is applied automatically when the entity implements `IBaseEntityUserId<TActor>` and a non-default actor value is provided.
- Soft-delete operations are available through the `SoftDelete` repository and service variants.
