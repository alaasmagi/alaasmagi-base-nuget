# alaasmagi Base Packages

Reusable base-layer building blocks for .NET web applications.

This package set provides shared abstractions and generic implementations for common application layers:

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

## What Is Included

These packages help standardize repetitive application code such as:

- base entity contracts with strongly typed IDs
- metadata support (`CreatedBy`, `CreatedAt`, `UpdatedBy`, `UpdatedAt`)
- soft-delete support
- user-owned entity support
- generic repository contracts
- generic EF Core repository implementations
- generic application service contracts and implementations
- mapper contracts
- method response and error models

## Typical Usage

A common setup looks like this:

1. Define your domain entity by inheriting from one of the `Base.Domain` classes.
2. Implement a mapper between your domain model and persistence model for the repository layer.
3. If your application layer uses separate DTOs or models, implement a second mapper between the application model and the domain model for the service layer.
4. Inherit from `BaseRepository<...>` or `BaseRepositorySoftDelete<...>`.
5. Inherit from `BaseService<...>` or `BaseServiceSoftDelete<...>`.
6. Use the contracts from `Base.Contracts.*` at your application boundaries.

## Example

```csharp
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
    public TodoRepository(AppDbContext dbContext, TodoMapper mapper)
        : base(dbContext, mapper)
    {
    }
}
```

## Target Framework

These packages currently target:

- `.NET 10.0`

Some packages also depend on:

- `Microsoft.EntityFrameworkCore`
- `Microsoft.Extensions.Caching.StackExchangeRedis` in `alaasmagi.Base.Domain`
- `Sentry` in `alaasmagi.Base.Domain`
- `Sentry.AspNetCore` in `alaasmagi.Base.Domain`

## Notes

- The packages are designed to be used together.
- Package versions should be kept in sync across the full `alaasmagi.Base.*` set.
- `Base.DataAccess.EF` is intended for EF Core-based persistence.
- `Base.Application` builds on repository and mapper abstractions from the contracts packages.
- Metadata timestamps are populated automatically by the EF repository base classes when the entity type implements `IBaseEntityMeta`.
- Metadata user identifiers are populated when a non-default `userId` is supplied to repository methods.
