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
/// <typeparam name="T">The type of the error code.</typeparam>
public interface IError<T>
    where T : IEquatable<T>
{
    /// <summary>
    /// Gets or sets the machine-readable error code.
    /// </summary>
    public T Code { get; set; }

    /// <summary>
    /// Gets or sets the human-readable error message.
    /// </summary>
    public string Message { get; set; }
}
