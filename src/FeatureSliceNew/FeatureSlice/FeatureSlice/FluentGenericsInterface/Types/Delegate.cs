using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;

namespace FeatureSlice.FluentGenerics.Interfaces;

public static class DelegateFeatureSlice
{
    public interface Default<TRequest, TResponse> : IFeatureSlice
    {
        public delegate Task<OneOf<TResponse, Error>> Dispatch(TRequest request);

        protected static void RegisterInternal<TDispatch>(IServiceCollection services, Func<IServiceProvider, Dispatch> dispatcher, Func<Dispatch, TDispatch> converter)
            where TDispatch : Delegate
        {
            services.AddSingleton<TDispatch>(provider => converter(dispatcher(provider)));
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

    public interface Flag<TRequest, TResponse> : IFeatureSlice
    {       
        public delegate Task<OneOf<TResponse, Disabled, Error>> Dispatch(TRequest request);

        protected static void RegisterInternal<TDispatch, TFlag>(IServiceCollection services, Func<IServiceProvider, Dispatch> dispatcher, Func<Dispatch, TDispatch> converter)
            where TDispatch : Delegate
            where TFlag : IFeatureFlag
        {
            services.AddSingleton<TDispatch>(provider => converter(dispatcher(provider)));
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