using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;

namespace FeatureSlice.FluentGenerics;

public partial interface IFeatureSlice
{
    public static class Delegate
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

    public static class Handler
    {
        public abstract partial class Default<TRequest, TResponse, THandler> : Delegate.Default<TRequest, TResponse>
            where THandler : class, IHandler<TRequest, TResponse>
        {
            protected static void RegisterInternal(IServiceCollection services)
            {
                services.AddSingleton<THandler>();
                RegisterInternal(services, provider => request => InMemoryDispatcher.Dispatch<TRequest, TResponse, THandler>(request, provider));
            }
        }

        public abstract partial class Flag<TFeatureFlag, TRequest, TResponse, THandler> : Delegate.Flag<TFeatureFlag, TRequest, TResponse>
            where TFeatureFlag : IFeatureFlag
            where THandler : class, IHandler<TRequest, TResponse>
        {
            protected static void RegisterInternal(IServiceCollection services)
            {
                services.AddSingleton<THandler>();
                RegisterInternal(services, provider => request => InMemoryDispatcher.WithFlag<TFeatureFlag>.Dispatch<TRequest, TResponse, THandler>(request, provider));
            }
        }
    }

    public static class Consumer
    {
        public abstract partial class Default<TRequest, TConsumer> : Delegate.Default<TRequest, Success>
            where TConsumer : class, IConsumer<TRequest>
        {
            protected static void RegisterInternal(IServiceCollection services)
            {
                services.AddSingleton<TConsumer>();
                RegisterInternal(services, provider => request => InMemoryDispatcher.Dispatch<TRequest, TResponse, TConsumer>(request, provider));
            }
        }

        public abstract partial class Flag<TFeatureFlag, TRequest, TConsumer> : Delegate.Flag<TFeatureFlag, TRequest, Success>
            where TFeatureFlag : IFeatureFlag
            where TConsumer : class, IConsumer<TRequest>
        {
            protected static void RegisterInternal(IServiceCollection services)
            {
                services.AddSingleton<TConsumer>();
                RegisterInternal(services, provider => request => InMemoryDispatcher.WithFlag<TFeatureFlag>.Dispatch<TRequest, TResponse, TConsumer>(request, provider));
            }
        }
    }

    public static class Endpoint
    {
        public abstract partial class Default<TEndpoint> : IFeatureSlice
            where TEndpoint : IEndpoint
        {
            protected static void RegisterInternal(HostExtender<WebApplication> hostExtender)
            {
                hostExtender.Map<TEndpoint>();
            }
        }

        public abstract partial class Flag<TFeatureFlag, TEndpoint> : IFeatureSlice
            where TFeatureFlag : IFeatureFlag
            where TEndpoint : IEndpoint
        {
            protected static void RegisterInternal(HostExtender<WebApplication> hostExtender)
            {
                hostExtender.Map<TEndpoint>();
            }
        }
    }
}