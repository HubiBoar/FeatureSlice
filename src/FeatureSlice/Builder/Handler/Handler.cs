using Microsoft.Extensions.DependencyInjection;
using Definit.Dependencies;
using Definit.Results;

namespace FeatureSlice;

public abstract class HandlerBase<TSelf, TRequest, TResponse, TDependencies> : Dispatchable<TSelf, TRequest, Result<TResponse>>
    where TSelf : HandlerBase<TSelf, TRequest, TResponse, TDependencies>
    where TDependencies : class, IFromServices<TDependencies>
    where TRequest : notnull
    where TResponse : notnull
{
    protected virtual ServiceLifetime ServiceLifetime { get; } = ServiceLifetime.Singleton;

    protected abstract Task<Result<TResponse>> Handle(TRequest request, TDependencies dependencies);
}

public abstract class HandlerBaseWithFlag<TSelf, TRequest, TResponse, TDependencies> : Dispatchable<TSelf, TRequest, Result<TResponse, Disabled>>
    where TSelf : HandlerBaseWithFlag<TSelf, TRequest, TResponse, TDependencies>
    where TDependencies : class, IFromServices<TDependencies>
    where TRequest : notnull
    where TResponse : notnull
{
    protected abstract string FeatureName { get; }

    protected virtual ServiceLifetime ServiceLifetime { get; } = ServiceLifetime.Singleton;

    protected abstract Task<Result<TResponse>> Handle(TRequest request, TDependencies dependencies);
}


public abstract class HandlerBase<TSelf, TRequest, TDependencies> : Dispatchable<TSelf, TRequest, Result>
    where TSelf : HandlerBase<TSelf, TRequest, TDependencies>
    where TDependencies : class, IFromServices<TDependencies>
    where TRequest : notnull
{
    protected virtual ServiceLifetime ServiceLifetime { get; } = ServiceLifetime.Singleton;

    protected abstract Task<Result> Handle(TRequest request, TDependencies dependencies);
}

public abstract class HandlerBaseWithFlag<TSelf, TRequest, TDependencies> : Dispatchable<TSelf, TRequest, Result.Or<Disabled>>
    where TSelf : HandlerBaseWithFlag<TSelf, TRequest, TDependencies>
    where TDependencies : class, IFromServices<TDependencies>
    where TRequest : notnull
{
    protected abstract string FeatureName { get; }

    protected virtual ServiceLifetime ServiceLifetime { get; } = ServiceLifetime.Singleton;

    protected abstract Task<Result> Handle(TRequest request, TDependencies dependencies);
}
