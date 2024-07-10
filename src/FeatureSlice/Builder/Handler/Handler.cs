using Microsoft.Extensions.DependencyInjection;
using Definit.Dependencies;
using Definit.Results;

namespace FeatureSlice;

public abstract class HandlerBase<TSelf, TRequest, TResponse, TDependencies>
    where TSelf : HandlerBase<TSelf, TRequest, TResponse, TDependencies>
    where TDependencies : class, IFromServices<TDependencies>
    where TRequest : notnull
    where TResponse : notnull
{
    public delegate Task<Result<TResponse>> Handle(TRequest request);

    protected virtual ServiceLifetime ServiceLifetime { get; } = ServiceLifetime.Singleton;

    protected abstract Task<Result<TResponse>> OnRequest(TRequest request, TDependencies dependencies);
}

public abstract class ConsumerBase<TSelf, TRequest, TResponse, TDependencies> :
        HandlerBase<TSelf, TRequest, TResponse, TDependencies>
    where TSelf : ConsumerBase<TSelf, TRequest, TResponse, TDependencies>
    where TDependencies : class, IFromServices<TDependencies>
    where TRequest : notnull
    where TResponse : notnull
{
    public delegate Task<Result> Consume(TRequest request);
}   