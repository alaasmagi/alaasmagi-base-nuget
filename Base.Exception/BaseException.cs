using Base.Contracts.Exception;

namespace Base.Exception;

/// <summary>
/// Represents a base exception with a string-based code.
/// </summary>
public class BaseException : BaseException<string>, IBaseException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseException"/> class.
    /// </summary>
    /// <param name="code">The machine-readable exception code.</param>
    /// <param name="message">The human-readable exception message.</param>
    public BaseException(string code, string message)
        : base(code, message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseException"/> class.
    /// </summary>
    /// <param name="code">The machine-readable exception code.</param>
    /// <param name="message">The human-readable exception message.</param>
    /// <param name="innerException">The exception that caused the current exception.</param>
    public BaseException(string code, string message, System.Exception? innerException)
        : base(code, message, innerException)
    {
    }
}

/// <summary>
/// Represents a base exception with a strongly typed code.
/// </summary>
/// <typeparam name="TCode">The type of the exception code.</typeparam>
public class BaseException<TCode> : System.Exception, IBaseException<TCode>
    where TCode : IEquatable<TCode>
{
    /// <summary>
    /// Gets the machine-readable exception code.
    /// </summary>
    public virtual TCode Code { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseException{TCode}"/> class.
    /// </summary>
    /// <param name="code">The machine-readable exception code.</param>
    /// <param name="message">The human-readable exception message.</param>
    public BaseException(TCode code, string message)
        : base(message)
    {
        Code = code;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseException{TCode}"/> class.
    /// </summary>
    /// <param name="code">The machine-readable exception code.</param>
    /// <param name="message">The human-readable exception message.</param>
    /// <param name="innerException">The exception that caused the current exception.</param>
    public BaseException(TCode code, string message, System.Exception? innerException)
        : base(message, innerException)
    {
        Code = code;
    }
}