using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;

namespace FeatureSlice;

public static class DelegateFeatureSlice
{
    public abstract partial class Default<TRequest, TResponse> : IFeatureSlice
    {
        public delegate Task<OneOf<TResponse, Error>> Dispatch(TRequest request);

        protected static void RegisterBase(IServiceCollection services, ServiceFactory<Dispatch> dispatcher)
        {
            services.AddSingleton<Dispatch>(provider => dispatcher(provider));

            Publisher<TRequest>.Register(services);
            Publisher<TRequest>.RegisterListener(services, RegisterListener);
            Publisher<TRequest>.Listen RegisterListener(IServiceProvider provider)
            {
                return async request => {
                    var result = await dispatcher(provider)(request);
                    return result.Match<OneOf<Success, Error>>(success => new Success(), error => error);
                };
            }
        }
    }

    public abstract partial class Flag<TRequest, TResponse> : IFeatureSlice
    {
        public delegate Task<OneOf<TResponse, Disabled, Error>> Dispatch(TRequest request);

        protected static void RegisterBase(IServiceCollection services, ServiceFactory<Dispatch> dispatcher)
        {
            services.AddSingleton<Dispatch>(provider => dispatcher(provider));

            Publisher<TRequest>.Register(services);
            Publisher<TRequest>.RegisterListener(services, RegisterListener);
            Publisher<TRequest>.Listen RegisterListener(IServiceProvider provider)
            {
                return async request => {
                    var result = await dispatcher(provider)(request);
                    return result.Match<OneOf<Success, Error>>(success => new Success(), disabled => new Success(), error => error);
                };
            }
        }
    }
}