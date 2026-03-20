namespace Base.Contracts.DTO;

/// <summary>
/// Represents the outcome of an operation using the default <see cref="IError"/> contract.
/// </summary>
/// <typeparam name="TValue">The type of the successful return value.</typeparam>
public interface IMethodResponse<TValue> : IMethodResponse<TValue, IError>
{
}

/// <summary>
/// Represents the outcome of an operation with either a value or an error.
/// </summary>
/// <typeparam name="TValue">The type of the successful return value.</typeparam>
/// <typeparam name="TError">The type of the error payload.</typeparam>
public interface IMethodResponse<TValue, TError>
{
    /// <summary>
    /// Gets a value indicating whether the operation completed successfully.
    /// </summary>
    public bool Successful { get; }

    /// <summary>
    /// Gets the successful return value when <see cref="Successful"/> is <see langword="true"/>.
    /// </summary>
    public TValue? Value { get; }

    /// <summary>
    /// Gets the error payload when <see cref="Successful"/> is <see langword="false"/>.
    /// </summary>
    public TError? Error { get; }
}
