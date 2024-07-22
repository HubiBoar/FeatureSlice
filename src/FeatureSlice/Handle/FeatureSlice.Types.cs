
using Microsoft.Extensions.DependencyInjection;
using Definit.Results;

namespace FeatureSlice;

public abstract class FeatureSlice<TSelf, TRequest, TResponse> : FeatureSliceBase<TSelf, TRequest, Result<TResponse>, TResponse>
    where TSelf : FeatureSlice<TSelf, TRequest, TResponse>, new()
    where TRequest : notnull
    where TResponse : notnull
{
    protected sealed override Result<TResponse> OnException(Exception exception)
    {
        return exception;
    }
}

public abstract class FeatureSlice<TSelf, TRequest> : FeatureSliceBase<TSelf, TRequest, Result, Success>
    where TSelf : FeatureSlice<TSelf, TRequest>, new()
    where TRequest : notnull
{
    protected sealed override Result OnException(Exception exception)
    {
        return exception;
    }
}