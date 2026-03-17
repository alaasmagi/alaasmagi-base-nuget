using Base.Contracts.DTO;
using Base.Domain;

namespace Base.DTO;

public class MethodResponse<TValue> : IMethodResponse<TValue> where TValue : class
{
    public bool Successful { get; }
    public TValue? Value { get; }
    public IError? Error { get; }

    private MethodResponse(TValue value)
    {
        Successful = true;
        Value = value;
    }

    private MethodResponse(IError error)
    {
        Successful = false;
        Error = error;
    }

    public static MethodResponse<TValue> Success(TValue value) => new(value);
    public static MethodResponse<TValue> Failure(IError error) => new(error);
}

public class MethodResponse<TValue, TError> : IMethodResponse<TValue, TError> where TValue : class where TError : IError
{
    public bool Successful { get; }
    public TValue? Value { get; }
    public TError? Error { get; }

    private MethodResponse(TValue value)
    {
        Successful = true;
        Value = value;
    }

    private MethodResponse(IError error)
    {
        Successful = false;
        Error = (TError?)(object?)error;
    }

    public static MethodResponse<TValue, TError> Success(TValue value) => new(value);
    public static MethodResponse<TValue, TError> Failure(IError error) => new(error);
}