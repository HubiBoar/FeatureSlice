using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;
using Definit.Dependencies;

namespace FeatureSlice;

public abstract class HandlerBase<TSelf, TRequest, TResponse, TDependencies> : Dispatchable<TSelf, TRequest, TResponse>
    where TSelf : HandlerBase<TSelf, TRequest, TResponse, TDependencies>
    where TDependencies : class, IFromServices<TDependencies>
    where TRequest : notnull
    where TResponse : notnull
{
    protected virtual ServiceLifetime ServiceLifetime { get; } = ServiceLifetime.Singleton;

    protected abstract Task<OneOf<TResponse, Error>> Handle(TRequest request, TDependencies dependencies);
}

public abstract class HandlerBaseWithFlag<TSelf, TRequest, TResponse, TDependencies> : DispatchableWithFlag<TSelf, TRequest, TResponse>
    where TSelf : HandlerBaseWithFlag<TSelf, TRequest, TResponse, TDependencies>
    where TDependencies : class, IFromServices<TDependencies>
    where TRequest : notnull
    where TResponse : notnull
{
    protected abstract string FeatureName { get; }

    protected virtual ServiceLifetime ServiceLifetime { get; } = ServiceLifetime.Singleton;

    protected abstract Task<OneOf<TResponse, Error>> Handle(TRequest request, TDependencies dependencies);
}
