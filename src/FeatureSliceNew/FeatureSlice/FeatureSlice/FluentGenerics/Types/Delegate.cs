using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;

namespace FeatureSlice.FluentGenerics;

public static class DelegateFeatureSlice
{
    public abstract partial class Default<TRequest, TResponse> : IFeatureSlice
    {
        public delegate Task<OneOf<TResponse, Error>> Dispatch(TRequest request);

        protected static void RegisterInternal(IServiceCollection services, Func<IServiceProvider, Dispatch> dispatcher)
        {
            services.AddSingleton<Dispatch>(provider => dispatcher(provider));
            services.AddSingleton<Publisher<TRequest>.Listen>(RegisterListener);
            Publisher<TRequest>.Listen RegisterListener(IServiceProvider provider)
            {
                return async request => {
                    var result = await dispatcher(provider)(request);
                    return result.Match<OneOf<Success, Error>>(success => new Success(), error => error);
                };
            }
        }
    }

    public abstract partial class Flag<TFeatureFlag, TRequest, TResponse> : IFeatureSlice
        where TFeatureFlag : IFeatureFlag
    {
        public delegate Task<OneOf<TResponse, Disabled, Error>> Dispatch(TRequest request);

        protected static void RegisterInternal(IServiceCollection services, Func<IServiceProvider, Dispatch> dispatcher)
        {
            services.AddSingleton<Dispatch>(provider => dispatcher(provider));
            services.AddSingleton<Publisher<TRequest>.Listen>(RegisterListener);
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