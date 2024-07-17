using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

public abstract partial class FeatureSliceBase<TSelf, TRequest, TResult, TResponse>
{
    public sealed partial record HandleSetup
    (
        Func<IServiceProvider, Dispatch<TRequest, TResult, TResponse>> DispatchFactory,
        ServiceLifetime ServiceLifetime
    )
    : ISetup
    {
        public Func<IServiceProvider, IDispatcher<TRequest, TResult, TResponse>> DispatcherFactory { get; set; } = DefaultDispatcher;

        private readonly List<Action<IServiceCollection>> _extensions = [];

        public void Extend(Action<IServiceCollection> extension)
        {
            _extensions.Add(extension);
        }

        public static IDispatcher<TRequest, TResult, TResponse> DefaultDispatcher(IServiceProvider provider)
        {
            return provider.GetRequiredService<IDispatcher<TRequest, TResult, TResponse>>();
        }

        public void Register(IServiceCollection services)
        {
            foreach(var extension in _extensions)
            {
                extension(services);
            }
            
            services.Add(ServiceDescriptor.Describe(typeof(Dispatch), GetDispatch, ServiceLifetime));

            Dispatch GetDispatch(IServiceProvider provider)
            {
                var dispatch = DispatcherFactory(provider)
                    .GetDispatcher
                    (
                        provider,
                        DispatchFactory(provider)
                    );
                
                return request => dispatch(request);
            }
        }
    }

    public static HandleSetup Handle<TDep0>
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
            lifetime
        );
    }

    public static HandleSetup Handle<TDep0, TDep1>
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
            lifetime
        );
    }
}