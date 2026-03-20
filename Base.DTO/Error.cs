using Base.Contracts.DTO;

namespace Base.DTO;

/// <summary>
/// Represents a concrete error payload with a string-based code and message.
/// </summary>
/// <param name="code">The machine-readable error code.</param>
/// <param name="message">The human-readable error message.</param>
public class Error(string code, string message) : IError
{
    /// <summary>
    /// Gets the machine-readable error code.
    /// </summary>
    public virtual string Code { get; } = code;

    /// <summary>
    /// Gets the human-readable error message.
    /// </summary>
    public virtual string Message { get; } = message;
}

/// <summary>
/// Represents a concrete error payload with a strongly typed error code and message.
/// </summary>
/// <typeparam name="TCode">The type of the error code.</typeparam>
/// <param name="code">The machine-readable error code.</param>
/// <param name="message">The human-readable error message.</param>
public class Error<TCode>(TCode code, string message) : IError<TCode>
    where TCode : IEquatable<TCode>
{
    /// <summary>
    /// Gets the machine-readable error code.
    /// </summary>
    public virtual TCode Code { get; } = code;

    /// <summary>
    /// Gets the human-readable error message.
    /// </summary>
    public virtual string Message { get; } = message;
}
