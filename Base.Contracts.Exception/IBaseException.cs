namespace Base.Contracts.Exception;

/// <summary>
/// Represents an exception with a string-based code.
/// </summary>
public interface IBaseException : IBaseException<string>
{
}

/// <summary>
/// Represents an exception with a strongly typed error code.
/// </summary>
/// <typeparam name="TCode">The type of the exception code.</typeparam>
public interface IBaseException<TCode> where TCode : IEquatable<TCode>
{
    /// <summary>
    /// Gets the machine-readable exception code.
    /// </summary>
    public TCode Code { get; }
    
    /// <summary>
    /// Gets the human-readable exception message.
    /// </summary>
    public string Message { get; }
}