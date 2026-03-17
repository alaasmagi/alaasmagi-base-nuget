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
2. Implement a mapper between your application model and persistence model.
3. Inherit from `BaseRepository<...>` or `BaseRepositorySoftDelete<...>`.
4. Inherit from `BaseService<...>` or `BaseServiceSoftDelete<...>`.
5. Use the contracts from `Base.Contracts.*` at your application boundaries.

## Example

```csharp
public class TodoEntity : BaseEntityWithMetaSoftDelete<Guid>
{
    public string Title { get; set; } = default!;
}

public class TodoRepository
    : BaseRepositorySoftDelete<TodoEntity, Guid, Guid>
{
    public TodoRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}
```

## Target Framework

These packages currently target:

- `.NET 10.0`

Some packages also depend on:

- `Microsoft.EntityFrameworkCore`

## Notes

- The packages are designed to be used together.
- Package versions should be kept in sync across the full `alaasmagi.Base.*` set.
- `Base.DataAccess.EF` is intended for EF Core-based persistence.
- `Base.Application` builds on repository and mapper abstractions from the contracts packages.
