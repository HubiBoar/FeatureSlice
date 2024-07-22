using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

public abstract partial class FeatureSliceBase<TSelf, TRequest, TResult, TResponse>
{
    protected abstract TResult OnException(Exception exception);

    protected HandleSetup Handle<TDep0>
    (
        Func<TRequest, TDep0, Task<TResult>> handle,
        Func<IServiceProvider, TDep0> factory,
        ServiceLifetime lifetime = ServiceLifetime.Transient
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
                        factory(provider)
                    ),
            OnException,
            lifetime
        );
    }

    protected HandleSetup Handle<TDep0>
    (
        Func<TRequest, TDep0, Task<TResult>> handle,
        ServiceLifetime lifetime = ServiceLifetime.Transient
    )
        where TDep0 : notnull
    {
        return Handle
        (
            handle,
            provider => provider.GetRequiredService<TDep0>(),
            lifetime
        );
    }

    protected HandleSetup Handle<TDep0, TDep1>
    (
        Func<TRequest, TDep0, TDep1, Task<TResult>> handle,
        ServiceLifetime lifetime = ServiceLifetime.Transient
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

    protected HandleSetup Handle<TDep0, TDep1, TDep2>
    (
        Func<TRequest, TDep0, TDep1, TDep2, Task<TResult>> handle,
        ServiceLifetime lifetime = ServiceLifetime.Transient
    )
        where TDep0 : notnull
        where TDep1 : notnull
        where TDep2 : notnull
    {
        return new HandleSetup
        (
            provider =>
                request =>
                    handle
                    (
                        request,
                        provider.GetRequiredService<TDep0>(), 
                        provider.GetRequiredService<TDep1>(),
                        provider.GetRequiredService<TDep2>()
                    ),
            OnException,
            lifetime
        );
    }

    protected HandleSetup Handle<TDep0, TDep1, TDep2, TDep3>
    (
        Func<TRequest, TDep0, TDep1, TDep2, TDep3, Task<TResult>> handle,
        ServiceLifetime lifetime = ServiceLifetime.Transient
    )
        where TDep0 : notnull
        where TDep1 : notnull
        where TDep2 : notnull
        where TDep3 : notnull
    {
        return new HandleSetup
        (
            provider =>
                request =>
                    handle
                    (
                        request,
                        provider.GetRequiredService<TDep0>(), 
                        provider.GetRequiredService<TDep1>(),
                        provider.GetRequiredService<TDep2>(),
                        provider.GetRequiredService<TDep3>()
                    ),
            OnException,
            lifetime
        );
    }

    protected HandleSetup Handle<TDep0, TDep1, TDep2, TDep3, TDep4>
    (
        Func<TRequest, TDep0, TDep1, TDep2, TDep3, TDep4, Task<TResult>> handle,
        ServiceLifetime lifetime = ServiceLifetime.Transient
    )
        where TDep0 : notnull
        where TDep1 : notnull
        where TDep2 : notnull
        where TDep3 : notnull
        where TDep4 : notnull
    {
        return new HandleSetup
        (
            provider =>
                request =>
                    handle
                    (
                        request,
                        provider.GetRequiredService<TDep0>(), 
                        provider.GetRequiredService<TDep1>(),
                        provider.GetRequiredService<TDep2>(),
                        provider.GetRequiredService<TDep3>(),
                        provider.GetRequiredService<TDep4>()
                    ),
            OnException,
            lifetime
        );
    }

    protected HandleSetup Handle<TDep0, TDep1, TDep2, TDep3, TDep4, TDep5>
    (
        Func<TRequest, TDep0, TDep1, TDep2, TDep3, TDep4, TDep5, Task<TResult>> handle,
        ServiceLifetime lifetime = ServiceLifetime.Transient
    )
        where TDep0 : notnull
        where TDep1 : notnull
        where TDep2 : notnull
        where TDep3 : notnull
        where TDep4 : notnull
        where TDep5 : notnull
    {
        return new HandleSetup
        (
            provider =>
                request =>
                    handle
                    (
                        request,
                        provider.GetRequiredService<TDep0>(), 
                        provider.GetRequiredService<TDep1>(),
                        provider.GetRequiredService<TDep2>(),
                        provider.GetRequiredService<TDep3>(),
                        provider.GetRequiredService<TDep4>(),
                        provider.GetRequiredService<TDep5>()
                    ),
            OnException,
            lifetime
        );
    }

    protected HandleSetup Handle<TDep0, TDep1, TDep2, TDep3, TDep4, TDep5, TDep6>
    (
        Func<TRequest, TDep0, TDep1, TDep2, TDep3, TDep4, TDep5, TDep6, Task<TResult>> handle,
        ServiceLifetime lifetime = ServiceLifetime.Transient
    )
        where TDep0 : notnull
        where TDep1 : notnull
        where TDep2 : notnull
        where TDep3 : notnull
        where TDep4 : notnull
        where TDep5 : notnull
        where TDep6 : notnull
    {
        return new HandleSetup
        (
            provider =>
                request =>
                    handle
                    (
                        request,
                        provider.GetRequiredService<TDep0>(), 
                        provider.GetRequiredService<TDep1>(),
                        provider.GetRequiredService<TDep2>(),
                        provider.GetRequiredService<TDep3>(),
                        provider.GetRequiredService<TDep4>(),
                        provider.GetRequiredService<TDep5>(),
                        provider.GetRequiredService<TDep6>()
                    ),
            OnException,
            lifetime
        );
    }

    protected HandleSetup Handle<TDep0, TDep1, TDep2, TDep3, TDep4, TDep5, TDep6, TDep7>
    (
        Func<TRequest, TDep0, TDep1, TDep2, TDep3, TDep4, TDep5, TDep6, TDep7, Task<TResult>> handle,
        ServiceLifetime lifetime = ServiceLifetime.Transient
    )
        where TDep0 : notnull
        where TDep1 : notnull
        where TDep2 : notnull
        where TDep3 : notnull
        where TDep4 : notnull
        where TDep5 : notnull
        where TDep6 : notnull
        where TDep7 : notnull
    {
        return new HandleSetup
        (
            provider =>
                request =>
                    handle
                    (
                        request,
                        provider.GetRequiredService<TDep0>(), 
                        provider.GetRequiredService<TDep1>(),
                        provider.GetRequiredService<TDep2>(),
                        provider.GetRequiredService<TDep3>(),
                        provider.GetRequiredService<TDep4>(),
                        provider.GetRequiredService<TDep5>(),
                        provider.GetRequiredService<TDep6>(),
                        provider.GetRequiredService<TDep7>()
                    ),
            OnException,
            lifetime
        );
    }

    protected HandleSetup Handle<TDep0, TDep1, TDep2, TDep3, TDep4, TDep5, TDep6, TDep7, TDep8>
    (
        Func<TRequest, TDep0, TDep1, TDep2, TDep3, TDep4, TDep5, TDep6, TDep7, TDep8, Task<TResult>> handle,
        ServiceLifetime lifetime = ServiceLifetime.Transient
    )
        where TDep0 : notnull
        where TDep1 : notnull
        where TDep2 : notnull
        where TDep3 : notnull
        where TDep4 : notnull
        where TDep5 : notnull
        where TDep6 : notnull
        where TDep7 : notnull
        where TDep8 : notnull
    {
        return new HandleSetup
        (
            provider =>
                request =>
                    handle
                    (
                        request,
                        provider.GetRequiredService<TDep0>(), 
                        provider.GetRequiredService<TDep1>(),
                        provider.GetRequiredService<TDep2>(),
                        provider.GetRequiredService<TDep3>(),
                        provider.GetRequiredService<TDep4>(),
                        provider.GetRequiredService<TDep5>(),
                        provider.GetRequiredService<TDep6>(),
                        provider.GetRequiredService<TDep7>(),
                        provider.GetRequiredService<TDep8>()
                    ),
            OnException,
            lifetime
        );
    }
}