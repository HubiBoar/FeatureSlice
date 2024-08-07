using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

public abstract partial record FeatureSliceBase<TRequest, TResult, TResponse, TFromException>
{
    protected static Setup Handle<TDep0>
    (
        Func<TRequest, TDep0, Task<TResult>> handle,
        Func<IServiceProvider, TDep0> factory,
        ServiceLifetime lifetime = ServiceLifetime.Transient
    )
        where TDep0 : notnull
    {
        return new Setup
        (
            provider =>
                request =>
                    handle
                    (
                        request,
                        factory(provider)
                    ),
            lifetime
        );
    }

    protected static Setup Handle
    (
        Func<TRequest, Task<TResult>> handle,
        ServiceLifetime lifetime = ServiceLifetime.Transient
    )
    {
        return Handle
        (
            (request, _) => handle(request),
            provider => provider,
            lifetime
        );
    }

    protected static Setup Handle<TDep0>
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

    protected static Setup Handle<TDep0, TDep1>
    (
        Func<TRequest, TDep0, TDep1, Task<TResult>> handle,
        ServiceLifetime lifetime = ServiceLifetime.Transient
    )
        where TDep0 : notnull
        where TDep1 : notnull
    {
        return Handle
        (
            (request, deps) => handle(request, deps.Dep0, deps.Dep1),
            provider => 
            (
                Dep0: provider.GetRequiredService<TDep0>(),
                Dep1: provider.GetRequiredService<TDep1>()
            ),
            lifetime
        );
    }

    protected static Setup Handle<TDep0, TDep1, TDep2>
    (
        Func<TRequest, TDep0, TDep1, TDep2, Task<TResult>> handle,
        ServiceLifetime lifetime = ServiceLifetime.Transient
    )
        where TDep0 : notnull
        where TDep1 : notnull
        where TDep2 : notnull
    {
        return Handle
        (
            (request, deps) => handle(request, deps.Dep0, deps.Dep1, deps.Dep2),
            provider => 
            (
                Dep0: provider.GetRequiredService<TDep0>(),
                Dep1: provider.GetRequiredService<TDep1>(),
                Dep2: provider.GetRequiredService<TDep2>()
            ),
            lifetime
        );
    }

    protected static Setup Handle<TDep0, TDep1, TDep2, TDep3>
    (
        Func<TRequest, TDep0, TDep1, TDep2, TDep3, Task<TResult>> handle,
        ServiceLifetime lifetime = ServiceLifetime.Transient
    )
        where TDep0 : notnull
        where TDep1 : notnull
        where TDep2 : notnull
        where TDep3 : notnull
    {
        return Handle
        (
            (request, deps) => handle(request, deps.Dep0, deps.Dep1, deps.Dep2, deps.Dep3),
            provider => 
            (
                Dep0: provider.GetRequiredService<TDep0>(),
                Dep1: provider.GetRequiredService<TDep1>(),
                Dep2: provider.GetRequiredService<TDep2>(),
                Dep3: provider.GetRequiredService<TDep3>()
            ),
            lifetime
        );
    }

    protected static Setup Handle<TDep0, TDep1, TDep2, TDep3, TDep4>
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
        return Handle
        (
            (request, deps) => handle(request, deps.Dep0, deps.Dep1, deps.Dep2, deps.Dep3, deps.Dep4),
            provider => 
            (
                Dep0: provider.GetRequiredService<TDep0>(),
                Dep1: provider.GetRequiredService<TDep1>(),
                Dep2: provider.GetRequiredService<TDep2>(),
                Dep3: provider.GetRequiredService<TDep3>(),
                Dep4: provider.GetRequiredService<TDep4>()
            ),
            lifetime
        );
    }

    protected static Setup Handle<TDep0, TDep1, TDep2, TDep3, TDep4, TDep5>
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
        return Handle
        (
            (request, deps) => handle(request, deps.Dep0, deps.Dep1, deps.Dep2, deps.Dep3, deps.Dep4, deps.Dep5),
            provider => 
            (
                Dep0: provider.GetRequiredService<TDep0>(),
                Dep1: provider.GetRequiredService<TDep1>(),
                Dep2: provider.GetRequiredService<TDep2>(),
                Dep3: provider.GetRequiredService<TDep3>(),
                Dep4: provider.GetRequiredService<TDep4>(),
                Dep5: provider.GetRequiredService<TDep5>()
            ),
            lifetime
        );
    }

    protected static Setup Handle<TDep0, TDep1, TDep2, TDep3, TDep4, TDep5, TDep6>
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
        return Handle
        (
            (request, deps) => handle(request, deps.Dep0, deps.Dep1, deps.Dep2, deps.Dep3, deps.Dep4, deps.Dep5, deps.Dep6),
            provider => 
            (
                Dep0: provider.GetRequiredService<TDep0>(),
                Dep1: provider.GetRequiredService<TDep1>(),
                Dep2: provider.GetRequiredService<TDep2>(),
                Dep3: provider.GetRequiredService<TDep3>(),
                Dep4: provider.GetRequiredService<TDep4>(),
                Dep5: provider.GetRequiredService<TDep5>(),
                Dep6: provider.GetRequiredService<TDep6>()
            ),
            lifetime
        );
    }

    protected static Setup Handle<TDep0, TDep1, TDep2, TDep3, TDep4, TDep5, TDep6, TDep7>
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
        return Handle
        (
            (request, deps) => handle(request, deps.Dep0, deps.Dep1, deps.Dep2, deps.Dep3, deps.Dep4, deps.Dep5, deps.Dep6, deps.Dep7),
            provider => 
            (
                Dep0: provider.GetRequiredService<TDep0>(),
                Dep1: provider.GetRequiredService<TDep1>(),
                Dep2: provider.GetRequiredService<TDep2>(),
                Dep3: provider.GetRequiredService<TDep3>(),
                Dep4: provider.GetRequiredService<TDep4>(),
                Dep5: provider.GetRequiredService<TDep5>(),
                Dep6: provider.GetRequiredService<TDep6>(),
                Dep7: provider.GetRequiredService<TDep7>()
            ),
            lifetime
        );
    }

    protected static Setup Handle<TDep0, TDep1, TDep2, TDep3, TDep4, TDep5, TDep6, TDep7, TDep8>
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
        return Handle
        (
            (request, deps) => handle(request, deps.Dep0, deps.Dep1, deps.Dep2, deps.Dep3, deps.Dep4, deps.Dep5, deps.Dep6, deps.Dep7, deps.Dep8),
            provider => 
            (
                Dep0: provider.GetRequiredService<TDep0>(),
                Dep1: provider.GetRequiredService<TDep1>(),
                Dep2: provider.GetRequiredService<TDep2>(),
                Dep3: provider.GetRequiredService<TDep3>(),
                Dep4: provider.GetRequiredService<TDep4>(),
                Dep5: provider.GetRequiredService<TDep5>(),
                Dep6: provider.GetRequiredService<TDep6>(),
                Dep7: provider.GetRequiredService<TDep7>(),
                Dep8: provider.GetRequiredService<TDep8>()
            ),
            lifetime
        );
    }

    protected static Setup Handle<TDep0>
    (
        Func<TRequest, TDep0, TResult> handle,
        Func<IServiceProvider, TDep0> factory,
        ServiceLifetime lifetime = ServiceLifetime.Transient
    )
        where TDep0 : notnull
    {
        return new Setup
        (
            provider =>
                request =>
                    Task.FromResult
                    (
                        handle
                        (
                            request,
                            factory(provider)
                        )
                    ),
            lifetime
        );
    }

    protected static Setup Handle
    (
        Func<TRequest, TResult> handle,
        ServiceLifetime lifetime = ServiceLifetime.Transient
    )
    {
        return Handle
        (
            (request, _) => handle(request),
            provider => provider,
            lifetime
        );
    }

    protected static Setup Handle<TDep0>
    (
        Func<TRequest, TDep0, TResult> handle,
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

    protected static Setup Handle<TDep0, TDep1>
    (
        Func<TRequest, TDep0, TDep1, TResult> handle,
        ServiceLifetime lifetime = ServiceLifetime.Transient
    )
        where TDep0 : notnull
        where TDep1 : notnull
    {
        return Handle
        (
            (request, deps) => handle(request, deps.Dep0, deps.Dep1),
            provider => 
            (
                Dep0: provider.GetRequiredService<TDep0>(),
                Dep1: provider.GetRequiredService<TDep1>()
            ),
            lifetime
        );
    }

    protected static Setup Handle<TDep0, TDep1, TDep2>
    (
        Func<TRequest, TDep0, TDep1, TDep2, TResult> handle,
        ServiceLifetime lifetime = ServiceLifetime.Transient
    )
        where TDep0 : notnull
        where TDep1 : notnull
        where TDep2 : notnull
    {
        return Handle
        (
            (request, deps) => handle(request, deps.Dep0, deps.Dep1, deps.Dep2),
            provider => 
            (
                Dep0: provider.GetRequiredService<TDep0>(),
                Dep1: provider.GetRequiredService<TDep1>(),
                Dep2: provider.GetRequiredService<TDep2>()
            ),
            lifetime
        );
    }

    protected static Setup Handle<TDep0, TDep1, TDep2, TDep3>
    (
        Func<TRequest, TDep0, TDep1, TDep2, TDep3, TResult> handle,
        ServiceLifetime lifetime = ServiceLifetime.Transient
    )
        where TDep0 : notnull
        where TDep1 : notnull
        where TDep2 : notnull
        where TDep3 : notnull
    {
        return Handle
        (
            (request, deps) => handle(request, deps.Dep0, deps.Dep1, deps.Dep2, deps.Dep3),
            provider => 
            (
                Dep0: provider.GetRequiredService<TDep0>(),
                Dep1: provider.GetRequiredService<TDep1>(),
                Dep2: provider.GetRequiredService<TDep2>(),
                Dep3: provider.GetRequiredService<TDep3>()
            ),
            lifetime
        );
    }

    protected static Setup Handle<TDep0, TDep1, TDep2, TDep3, TDep4>
    (
        Func<TRequest, TDep0, TDep1, TDep2, TDep3, TDep4, TResult> handle,
        ServiceLifetime lifetime = ServiceLifetime.Transient
    )
        where TDep0 : notnull
        where TDep1 : notnull
        where TDep2 : notnull
        where TDep3 : notnull
        where TDep4 : notnull
    {
        return Handle
        (
            (request, deps) => handle(request, deps.Dep0, deps.Dep1, deps.Dep2, deps.Dep3, deps.Dep4),
            provider => 
            (
                Dep0: provider.GetRequiredService<TDep0>(),
                Dep1: provider.GetRequiredService<TDep1>(),
                Dep2: provider.GetRequiredService<TDep2>(),
                Dep3: provider.GetRequiredService<TDep3>(),
                Dep4: provider.GetRequiredService<TDep4>()
            ),
            lifetime
        );
    }

    protected static Setup Handle<TDep0, TDep1, TDep2, TDep3, TDep4, TDep5>
    (
        Func<TRequest, TDep0, TDep1, TDep2, TDep3, TDep4, TDep5, TResult> handle,
        ServiceLifetime lifetime = ServiceLifetime.Transient
    )
        where TDep0 : notnull
        where TDep1 : notnull
        where TDep2 : notnull
        where TDep3 : notnull
        where TDep4 : notnull
        where TDep5 : notnull
    {
        return Handle
        (
            (request, deps) => handle(request, deps.Dep0, deps.Dep1, deps.Dep2, deps.Dep3, deps.Dep4, deps.Dep5),
            provider => 
            (
                Dep0: provider.GetRequiredService<TDep0>(),
                Dep1: provider.GetRequiredService<TDep1>(),
                Dep2: provider.GetRequiredService<TDep2>(),
                Dep3: provider.GetRequiredService<TDep3>(),
                Dep4: provider.GetRequiredService<TDep4>(),
                Dep5: provider.GetRequiredService<TDep5>()
            ),
            lifetime
        );
    }

    protected static Setup Handle<TDep0, TDep1, TDep2, TDep3, TDep4, TDep5, TDep6>
    (
        Func<TRequest, TDep0, TDep1, TDep2, TDep3, TDep4, TDep5, TDep6, TResult> handle,
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
        return Handle
        (
            (request, deps) => handle(request, deps.Dep0, deps.Dep1, deps.Dep2, deps.Dep3, deps.Dep4, deps.Dep5, deps.Dep6),
            provider => 
            (
                Dep0: provider.GetRequiredService<TDep0>(),
                Dep1: provider.GetRequiredService<TDep1>(),
                Dep2: provider.GetRequiredService<TDep2>(),
                Dep3: provider.GetRequiredService<TDep3>(),
                Dep4: provider.GetRequiredService<TDep4>(),
                Dep5: provider.GetRequiredService<TDep5>(),
                Dep6: provider.GetRequiredService<TDep6>()
            ),
            lifetime
        );
    }

    protected static Setup Handle<TDep0, TDep1, TDep2, TDep3, TDep4, TDep5, TDep6, TDep7>
    (
        Func<TRequest, TDep0, TDep1, TDep2, TDep3, TDep4, TDep5, TDep6, TDep7, TResult> handle,
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
        return Handle
        (
            (request, deps) => handle(request, deps.Dep0, deps.Dep1, deps.Dep2, deps.Dep3, deps.Dep4, deps.Dep5, deps.Dep6, deps.Dep7),
            provider => 
            (
                Dep0: provider.GetRequiredService<TDep0>(),
                Dep1: provider.GetRequiredService<TDep1>(),
                Dep2: provider.GetRequiredService<TDep2>(),
                Dep3: provider.GetRequiredService<TDep3>(),
                Dep4: provider.GetRequiredService<TDep4>(),
                Dep5: provider.GetRequiredService<TDep5>(),
                Dep6: provider.GetRequiredService<TDep6>(),
                Dep7: provider.GetRequiredService<TDep7>()
            ),
            lifetime
        );
    }

    protected static Setup Handle<TDep0, TDep1, TDep2, TDep3, TDep4, TDep5, TDep6, TDep7, TDep8>
    (
        Func<TRequest, TDep0, TDep1, TDep2, TDep3, TDep4, TDep5, TDep6, TDep7, TDep8, TResult> handle,
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
        return Handle
        (
            (request, deps) => handle(request, deps.Dep0, deps.Dep1, deps.Dep2, deps.Dep3, deps.Dep4, deps.Dep5, deps.Dep6, deps.Dep7, deps.Dep8),
            provider => 
            (
                Dep0: provider.GetRequiredService<TDep0>(),
                Dep1: provider.GetRequiredService<TDep1>(),
                Dep2: provider.GetRequiredService<TDep2>(),
                Dep3: provider.GetRequiredService<TDep3>(),
                Dep4: provider.GetRequiredService<TDep4>(),
                Dep5: provider.GetRequiredService<TDep5>(),
                Dep6: provider.GetRequiredService<TDep6>(),
                Dep7: provider.GetRequiredService<TDep7>(),
                Dep8: provider.GetRequiredService<TDep8>()
            ),
            lifetime
        );
    }
}