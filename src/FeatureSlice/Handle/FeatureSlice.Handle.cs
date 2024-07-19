using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

public abstract partial class FeatureSliceBase<TSelf, TRequest, TResult, TResponse>
{
    protected abstract TResult OnException(Exception exception);

    protected HandleSetup Handle<TDep0>
    (
        Func<TRequest, TDep0, Task<TResult>> handle,
        ServiceLifetime lifetime = ServiceLifetime.Singleton
    )
        where TDep0 : notnull
    {
        return new HandleSetup
        (
            provider =>
                request =>
                    handle
                    (
                        request,
                        provider.GetRequiredService<TDep0>()
                    ),
            OnException,
            lifetime
        );
    }

    protected HandleSetup Handle<TDep0, TDep1>
    (
        Func<TRequest, TDep0, TDep1, Task<TResult>> handle,
        ServiceLifetime lifetime = ServiceLifetime.Singleton
    )
        where TDep0 : notnull
        where TDep1 : notnull
    {
        return new HandleSetup
        (
            provider =>
                request =>
                    handle
                    (
                        request,
                        provider.GetRequiredService<TDep0>(), 
                        provider.GetRequiredService<TDep1>()
                    ),
            OnException,
            lifetime
        );
    }
}