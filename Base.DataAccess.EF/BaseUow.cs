using Base.Contracts.DataAccess;
using Base.Contracts.DTO;
using Base.DTO;
using Microsoft.EntityFrameworkCore;

namespace Base.DataAccess.EF;

/// <summary>
/// Provides a base Entity Framework unit-of-work implementation backed by a <see cref="DbContext"/>.
/// </summary>
/// <typeparam name="TDbContext">The concrete database context type.</typeparam>
public class BaseUow<TDbContext> : IBaseUow
    where TDbContext : DbContext
{
    /// <summary>
    /// Stores the database context used by the unit of work.
    /// </summary>
    protected readonly TDbContext UowDbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseUow{TDbContext}"/> class.
    /// </summary>
    /// <param name="uowDbContext">The database context used to persist changes.</param>
    public BaseUow(TDbContext uowDbContext)
    {
        UowDbContext = uowDbContext;
    }

    /// <summary>
    /// Persists all pending changes in the underlying database context.
    /// </summary>
    /// <returns>
    /// A task that resolves to the number of state entries written to the database.
    /// </returns>
    public virtual async Task<int> SaveChangesAsync()
    {
        return await UowDbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Persists all pending changes in the underlying database context. A <see cref="DbUpdateException"/> is
    /// returned as a failure (<see cref="ErrorDefaults.Codes.ConcurrencyConflict"/> for a concurrency violation,
    /// otherwise <see cref="ErrorDefaults.Codes.SaveFailed"/>) and the change tracker is cleared so the context
    /// can be reused. Any other exception propagates.
    /// </summary>
    /// <returns>
    /// A task that resolves to the number of state entries written, or a failure describing why the write was rejected.
    /// </returns>
    public virtual async Task<IMethodResponse<int>> TrySaveChangesAsync()
    {
        try
        {
            return MethodResponse<int>.Success(await UowDbContext.SaveChangesAsync());
        }
        catch (DbUpdateConcurrencyException)
        {
            UowDbContext.ChangeTracker.Clear();
            return MethodResponse<int>.Failure(
                new Error(ErrorDefaults.Codes.ConcurrencyConflict, ErrorDefaults.Messages.ConcurrencyConflict));
        }
        catch (DbUpdateException)
        {
            UowDbContext.ChangeTracker.Clear();
            return MethodResponse<int>.Failure(
                new Error(ErrorDefaults.Codes.SaveFailed, ErrorDefaults.Messages.SaveFailed));
        }
    }
}
