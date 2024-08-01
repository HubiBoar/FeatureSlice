using Definit.Results;

namespace FeatureSlice;

public abstract partial record FeatureSlice<TRequest, TResponse>
(
    IFeatureSliceSetup<TRequest, Result<TResponse>, TResponse> Options
)
: FeatureSliceBase<TRequest, Result<TResponse>, TResponse, ResultFromException<TResponse>>
(
    Options
)
    where TRequest : notnull
    where TResponse : notnull
{
}

public abstract partial record FeatureSlice<TRequest>
(
    IFeatureSliceSetup<TRequest, Result, Success> Options
)
: FeatureSliceBase<TRequest, Result, Success, ResultFromException>
(
    Options
)
    where TRequest : notnull
{
}

public sealed class ResultFromException<TResponse> : IFromException<Result<TResponse>, TResponse>
    where TResponse : notnull
{
    public static Result<TResponse> FromException(Exception exception)
    {
        return exception;
    }
}


public sealed class ResultFromException : IFromException<Result, Success>
{
    public static Result FromException(Exception exception)
    {
        return exception;
    }
}