namespace Base.Contracts.Exception;

/// <summary>
/// Represents a web exception with a string-based code.
/// </summary>
public interface IHttpException : IHttpException<string>
{
}

/// <summary>
/// Represents a web exception with a strongly typed code and HTTP status code.
/// </summary>
/// <typeparam name="TCode">The type of the exception code.</typeparam>
public interface IHttpException<TCode> : IBaseException<TCode> where TCode: IEquatable<TCode>
{
    /// <summary>
    /// Gets the HTTP status code associated with the exception.
    /// </summary>
    public int StatusCode { get; }
}