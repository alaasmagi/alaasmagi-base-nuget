using Base.Contracts.Exception;

namespace Base.Exception;

/// <summary>
/// Represents a web exception with a string-based code and HTTP status code.
/// </summary>
public class HttpException : HttpException<string>, IHttpException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HttpException"/> class.
    /// </summary>
    /// <param name="code">The machine-readable exception code.</param>
    /// <param name="message">The human-readable exception message.</param>
    /// <param name="statusCode">The HTTP status code associated with the exception.</param>
    public HttpException(string code, string message, int statusCode)
        : base(code, message, statusCode)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpException"/> class.
    /// </summary>
    /// <param name="code">The machine-readable exception code.</param>
    /// <param name="message">The human-readable exception message.</param>
    /// <param name="statusCode">The HTTP status code associated with the exception.</param>
    /// <param name="innerException">The exception that caused the current exception.</param>
    public HttpException(string code, string message, int statusCode, System.Exception? innerException)
        : base(code, message, statusCode, innerException)
    {
    }
}

/// <summary>
/// Represents a web exception with a strongly typed code and HTTP status code.
/// </summary>
/// <typeparam name="TCode">The type of the exception code.</typeparam>
public class HttpException<TCode> : BaseException<TCode>, IHttpException<TCode>
    where TCode : IEquatable<TCode>
{
    /// <summary>
    /// Gets the HTTP status code associated with the exception.
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpException{TCode}"/> class.
    /// </summary>
    /// <param name="code">The machine-readable exception code.</param>
    /// <param name="message">The human-readable exception message.</param>
    /// <param name="statusCode">The HTTP status code associated with the exception.</param>
    public HttpException(TCode code, string message, int statusCode)
        : base(code, message)
    {
        StatusCode = statusCode;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpException{TCode}"/> class.
    /// </summary>
    /// <param name="code">The machine-readable exception code.</param>
    /// <param name="message">The human-readable exception message.</param>
    /// <param name="statusCode">The HTTP status code associated with the exception.</param>
    /// <param name="innerException">The exception that caused the current exception.</param>
    public HttpException(TCode code, string message, int statusCode, System.Exception? innerException)
        : base(code, message, innerException)
    {
        StatusCode = statusCode;
    }
}

