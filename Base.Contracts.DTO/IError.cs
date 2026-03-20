namespace Base.Contracts.DTO;

/// <summary>
/// Represents an error with a string-based code.
/// </summary>
public interface IError : IError<string>
{
}

/// <summary>
/// Represents an error with a strongly typed error code.
/// </summary>
/// <typeparam name="TCode">The type of the error code.</typeparam>
public interface IError<TCode>
    where TCode : IEquatable<TCode>
{
    /// <summary>
    /// Gets the machine-readable error code.
    /// </summary>
    public TCode Code { get; }

    /// <summary>
    /// Gets the human-readable error message.
    /// </summary>
    public string Message { get; }
}
