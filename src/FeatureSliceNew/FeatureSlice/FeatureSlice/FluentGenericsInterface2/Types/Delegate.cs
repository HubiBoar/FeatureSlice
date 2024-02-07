using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;

namespace FeatureSlice.FluentGenerics.Interfaces2;

public static class DelegateFeatureSlice
{
    public interface Default<TRequest, TResponse> : IFeatureSlice
    {
        public delegate Task<OneOf<TResponse, Error>> Dispatch(TRequest request);

        public delegate TDispatch DispatchConverter<TDispatch>(Dispatch dispatch)
            where TDispatch : Delegate;

        public static void RegisterBase<TDispatch>(IServiceCollection services, Func<IServiceProvider, Dispatch> dispatcher, DispatchConverter<TDispatch> converter)
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

        public delegate TDispatch DispatchConverter<TDispatch>(Dispatch dispatch)
            where TDispatch : Delegate;

        protected static void RegisterBase<TFlag, TDispatch>(IServiceCollection services, Func<IServiceProvider, Dispatch> dispatcher, DispatchConverter<TDispatch> converter)
            where TFlag : IFeatureFlag
            where TDispatch : Delegate
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