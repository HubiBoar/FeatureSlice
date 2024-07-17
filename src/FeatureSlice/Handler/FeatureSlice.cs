using Microsoft.Extensions.DependencyInjection;
using Definit.Results;

namespace FeatureSlice;

public static partial class FeatureSlice<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : notnull
{
    public abstract class Build<TSelf>
        where TSelf : HandlerBase<TSelf, TRequest, Result<TResponse>, TResponse>, new()
    {
    }
}

public static partial class FeatureSlice<TRequest>
    where TRequest : notnull
{
    public abstract class Build<TSelf>
        where TSelf : HandlerBase<TSelf, TRequest, Result, Success>, new()
    {
    }
}

public abstract partial class HandlerBase<TSelf, TRequest, TResult, TResponse>
    where TSelf : HandlerBase<TSelf, TRequest, TResult, TResponse>, new()
    where TRequest : notnull
    where TResult : Result_Base<TResponse>
    where TResponse : notnull
{
    public delegate Task<TResult> Dispatch(TRequest request);

    public abstract Options Handler { get; }

    public static Options Handle<TDep0>(Func<TRequest, TDep0, Task<TResult>> handle, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TDep0 : notnull
    {
        return new Options
        (
            provider =>
                request =>
                    handle
                    (
                        request,
                        provider.GetRequiredService<TDep0>()
                    ),
            lifetime
        );
    }

    public static Options Handle<TDep0, TDep1>(Func<TRequest, TDep0, TDep1, Task<TResult>> handle, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TDep0 : notnull
        where TDep1 : notnull
    {
        return new Options
        (
            provider =>
                request =>
                    handle
                    (
                        request,
                        provider.GetRequiredService<TDep0>(), 
                        provider.GetRequiredService<TDep1>()
                    ),
            lifetime
        );
    }

    public static void Register
    (
        IServiceCollection services
    )
    {
        var self = new TSelf();

        self.Handler.Register(services);
    }
}
