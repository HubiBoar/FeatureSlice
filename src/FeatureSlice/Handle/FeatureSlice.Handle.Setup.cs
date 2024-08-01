using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FeatureSlice;

public abstract partial record FeatureSliceBase<TRequest, TResult, TResponse>
{
    public sealed partial record HandleSetup
    (
        Func<IServiceProvider, Dispatch<TRequest, TResult, TResponse>> DispatchFactory,
        Func<Exception, TResult> OnException,
        ServiceLifetime ServiceLifetime
    )
    : ISetup
    {
        public Func<IServiceProvider, Dispatcher<TRequest, TResult, TResponse>> DispatcherFactory { get; set; } = DefaultDispatcher;

        private readonly List<Action<IServiceCollection>> _extensions = [];

        public void Extend(Action<IServiceCollection> extension)
        {
            _extensions.Add(extension);
        }

        public static Dispatcher<TRequest, TResult, TResponse> DefaultDispatcher(IServiceProvider provider)
        {
            return provider.GetRequiredService<IDispatcher>().GetDispatcher;
        }

        public void Register(IServiceCollection services)
        {
            services.TryAdd(IDispatcher.RegisterDefault());

            foreach(var extension in _extensions)
            {
                extension(services);
            }

            services.Add(ServiceDescriptor.Describe(typeof(Dispatch), GetDispatchLocal, ServiceLifetime));

            Dispatch GetDispatchLocal(IServiceProvider provider)
            {
                var dispatch = DispatcherFactory(provider)
                (
                    provider,
                    DispatchFactory(provider)
                );

                return async request => 
                {
                    try
                    {
                        return await dispatch(request);
                    }
                    catch (Exception exception)
                    {
                        return OnException(exception);
                    }
                };
            }
        }

        public Dispatch<TRequest, TResult, TResponse> GetDispatch(IServiceProvider provider)
        {
            return request => provider.GetRequiredService<Dispatch>()(request);
        }
    }
}