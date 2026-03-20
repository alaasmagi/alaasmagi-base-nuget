# alaasmagi Base Packages

Reusable base-layer building blocks for .NET applications.

This package set provides shared contracts and generic implementations for common layers:

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

## Included Capabilities

These packages cover the common building blocks used across layered applications:

- strongly typed base entity contracts
- domain base classes with metadata, ownership, and soft delete support
- mapper abstractions between layers
- method-response and error models
- generic repository contracts
- generic EF Core repository implementations
- unit-of-work abstractions
- generic application service contracts and implementations
- typed base exceptions and HTTP exceptions

## Package Overview

### `alaasmagi.Base.Contracts.Domain`

Defines contracts such as:

- `IBaseEntity<TKey>`
- `IBaseEntityMeta`
- `IBaseEntitySoftDelete`
- `IBaseEntityUserId<TKey>`
- `IBaseEntityWithMeta<TKey>`
- `IBaseEntityWithMetaSoftDelete<TKey>`

### `alaasmagi.Base.Domain`

Provides reusable base entity types such as:

- `BaseEntity<TKey>`
- `BaseEntityWithMeta<TKey>`
- `BaseEntityWithMetaSoftDelete<TKey>`
- `BaseEntityUser<TKey>`
- `BaseEntityUserWithMeta<TKey, TUserKey>`
- `BaseEntityUserWithMetaSoftDelete<TKey, TUserKey>`

### `alaasmagi.Base.Contracts.DTO`

Defines:

- `IMapper<TUpperEntity, TLowerEntity, TKey>`
- `IMethodResponse<TValue>`
- `IError<TCode>`

### `alaasmagi.Base.DTO`

Provides:

- `MethodResponse<TValue>`
- `MethodResponse<TValue, TError>`
- `Error`
- `Error<TCode>`
- `ErrorDefaults`

### `alaasmagi.Base.Contracts.DataAccess`

Defines:

- `IBaseRepository<TEntity, TResourceKey, TActor>`
- `IBaseRepositorySoftDelete<TEntity, TResourceKey, TActor>`
- `IBaseUow`

### `alaasmagi.Base.DataAccess.EF`

Provides EF Core implementations such as:

- `BaseRepository<TDomainEntity, TDataAccessEntity, TMapper, TResourceKey, TActor>`
- `BaseRepositorySoftDelete<TDomainEntity, TDataAccessEntity, TMapper, TResourceKey, TActor>`
- `BaseUow<TDbContext>`

### `alaasmagi.Base.Contracts.Application`

Defines:

- `IBaseService<TEntity, TKey, TActor>`
- `IBaseServiceSoftDelete<TEntity, TKey, TActor>`
- `IBaseServiceUow`

### `alaasmagi.Base.Application`

Provides:

- `BaseService<TEntity, TDomainEntity, TRepository, TKey, TActor>`
- `BaseServiceSoftDelete<TEntity, TDomainEntity, TRepository, TKey, TActor>`
- `BaseServiceUow<TUow>`

### `alaasmagi.Base.Contracts.Exception`

Defines:

- `IBaseException<TCode>`
- `IHttpException<TCode>`

### `alaasmagi.Base.Exception`

Provides:

- `BaseException`
- `BaseException<TCode>`
- `HttpException`
- `HttpException<TCode>`

## Typical Usage

A common setup looks like this:

1. Inherit from a `Base.Domain` entity type that matches your needs.
2. Implement an `IMapper` between your domain model and EF model if those types differ.
3. Inherit from `BaseRepository<...>` or `BaseRepositorySoftDelete<...>`.
4. If your application layer uses different models, implement another mapper between application and domain models.
5. Inherit from `BaseService<...>` or `BaseServiceSoftDelete<...>`.

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

## Framework and Dependencies

All packages in this repository currently target:

- `.NET 10.0`

Relevant package dependencies used across the set include:

- `Microsoft.EntityFrameworkCore`
- `Microsoft.Extensions.Caching.StackExchangeRedis`
- `Sentry`
- `Sentry.AspNetCore`

## Notes

- These packages are designed to be used together, but each package can also be consumed independently where appropriate.
- Keep package versions in sync across the `alaasmagi.Base.*` set.
- `Base.DataAccess.EF` is intended for EF Core based persistence.
- Repository and service methods return `IMethodResponse<T>` so calling code can handle success and failure consistently.
- Metadata timestamps and actor fields are populated automatically by the EF repository base classes when supported by the entity type.
- User-based scoping is applied automatically when the entity implements `IBaseEntityUserId<TActor>` and a non-default actor value is supplied.
