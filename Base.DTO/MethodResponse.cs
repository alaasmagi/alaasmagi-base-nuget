using Base.Contracts.DTO;

namespace Base.DTO;

/// <summary>
/// Provides a concrete method response implementation that uses the default <see cref="IError"/> contract.
/// </summary>
/// <typeparam name="TValue">The type of the successful return value.</typeparam>
public class MethodResponse<TValue> : IMethodResponse<TValue>
    where TValue : class
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
    public IError? Error { get; }

    /// <summary>
    /// Initializes a successful response instance.
    /// </summary>
    /// <param name="value">The successful return value.</param>
    private MethodResponse(TValue value)
    {
        Successful = true;
        Value = value;
    }

    /// <summary>
    /// Initializes a failed response instance.
    /// </summary>
    /// <param name="error">The error payload describing the failure.</param>
    private MethodResponse(IError error)
    {
        Successful = false;
        Error = error;
    }

    /// <summary>
    /// Creates a successful response instance.
    /// </summary>
    /// <param name="value">The successful return value.</param>
    /// <returns>
    /// A successful <see cref="MethodResponse{TValue}"/> containing the provided value.
    /// </returns>
    public static MethodResponse<TValue> Success(TValue value) => new(value);

    /// <summary>
    /// Creates a failed response instance.
    /// </summary>
    /// <param name="error">The error payload describing the failure.</param>
    /// <returns>
    /// A failed <see cref="MethodResponse{TValue}"/> containing the provided error.
    /// </returns>
    public static MethodResponse<TValue> Failure(IError error) => new(error);
}

/// <summary>
/// Provides a concrete method response implementation that exposes a custom error interface type.
/// </summary>
/// <typeparam name="TValue">The type of the successful return value.</typeparam>
/// <typeparam name="TError">The type of the error payload.</typeparam>
public class MethodResponse<TValue, TError> : IMethodResponse<TValue, TError>
    where TValue : class
    where TError : IError
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
    /// The value is populated only when the provided error implements <typeparamref name="TError"/>.
    /// </summary>
    public TError? Error { get; }

    /// <summary>
    /// Initializes a successful response instance.
    /// </summary>
    /// <param name="value">The successful return value.</param>
    private MethodResponse(TValue value)
    {
        Successful = true;
        Value = value;
    }

    /// <summary>
    /// Initializes a failed response instance.
    /// </summary>
    /// <param name="error">The error payload describing the failure.</param>
    private MethodResponse(IError error)
    {
        Successful = false;
        Error = (TError?)(object?)error;
    }

    /// <summary>
    /// Creates a successful response instance.
    /// </summary>
    /// <param name="value">The successful return value.</param>
    /// <returns>
    /// A successful <see cref="MethodResponse{TValue, TError}"/> containing the provided value.
    /// </returns>
    public static MethodResponse<TValue, TError> Success(TValue value) => new(value);

    /// <summary>
    /// Creates a failed response instance.
    /// </summary>
    /// <param name="error">The error payload describing the failure.</param>
    /// <returns>
    /// A failed <see cref="MethodResponse{TValue, TError}"/> whose <see cref="Error"/> value is set when the provided error implements <typeparamref name="TError"/>.
    /// </returns>
    public static MethodResponse<TValue, TError> Failure(IError error) => new(error);
}
