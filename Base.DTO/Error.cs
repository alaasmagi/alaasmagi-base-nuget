using Base.Contracts.DTO;

namespace Base.DTO;

/// <summary>
/// Represents a concrete error payload with a strongly typed error code and message.
/// </summary>
/// <typeparam name="T">The type of the error code.</typeparam>
/// <param name="code">The machine-readable error code.</param>
/// <param name="message">The human-readable error message.</param>
public class Error<T>(T code, string message) : IError<T>
    where T : IEquatable<T>
{
    /// <summary>
    /// Gets or sets the machine-readable error code.
    /// </summary>
    public T Code { get; set; } = code;

    /// <summary>
    /// Gets or sets the human-readable error message.
    /// </summary>
    public string Message { get; set; } = message;
}
