using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice.FluentGenerics;

public static partial class FeatureSlice<TSelf>
    where TSelf : IFeatureSlice
{
    public abstract partial class WithHandler<TRequest, TResponse, THandler> : IFeatureSlice.Handler.Default<TRequest, TResponse, THandler>
        where THandler : class, IHandler<TRequest, TResponse>
    {
        public static void Register(IServiceCollection services)
        {
            RegisterInternal(services);
        }
    }

    public abstract partial class WithConsumer<TRequest, TConsumer> : IFeatureSlice.Consumer.Default<TRequest, TConsumer>
        where TConsumer : class, IConsumer<TRequest>
    {
        public static void Register(IServiceCollection services)
        {
            RegisterInternal(services);
        }
    }

    public abstract partial class WithEndpoint<TEndpoint> : IFeatureSlice.Endpoint.Default<TEndpoint>
        where TEndpoint : IEndpoint
    {
        public static void Register(HostExtender<WebApplication> hostExtender)
        {
            RegisterInternal(hostExtender);
        }

        public abstract partial class WithHandler<TRequest, TResponse, THandler> : IFeatureSlice.Handler.Default<TRequest, TResponse, THandler>
            where THandler : class, IHandler<TRequest, TResponse>
        {
            public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
            {
                WithEndpoint<TEndpoint>.Register(hostExtender);
                RegisterInternal(services);
            }
        }

        public abstract partial class WithConsumer<TRequest, TConsumer> : IFeatureSlice.Consumer.Default<TRequest, TConsumer>
            where TConsumer : class, IConsumer<TRequest>
        {
            public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
            {
                WithEndpoint<TEndpoint>.Register(hostExtender);
                RegisterInternal(services);
            }
        }
    }

    public static partial class WithFlag<TFeatureFlag>
        where TFeatureFlag : IFeatureFlag
    {
        public abstract partial class WithHandler<TRequest, TResponse, THandler> : IFeatureSlice.Handler.Flag<TFeatureFlag, TRequest, TResponse, THandler>
            where THandler : class, IHandler<TRequest, TResponse>
        {
            public static void Register(IServiceCollection services)
            {
                RegisterInternal(services);
            }
        }

        public abstract partial class WithConsumer<TRequest, TConsumer> : IFeatureSlice.Consumer.Flag<TFeatureFlag, TRequest, TConsumer>
            where TConsumer : class, IConsumer<TRequest>
        {
            public static void Register(IServiceCollection services)
            {
                RegisterInternal(services);
            }
        }

        public abstract partial class WithEndpoint<TEndpoint> : IFeatureSlice.Endpoint.Flag<TFeatureFlag, TEndpoint>
            where TEndpoint : IEndpoint
        {
            public static void Register(HostExtender<WebApplication> hostExtender)
            {
                RegisterInternal(hostExtender);
            }

            public abstract partial class WithHandler<TRequest, TResponse, THandler> : IFeatureSlice.Handler.Flag<TFeatureFlag, TRequest, TResponse, THandler>
                where THandler : class, IHandler<TRequest, TResponse>
            {
                public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
                {
                    WithEndpoint<TEndpoint>.Register(hostExtender);
                    RegisterInternal(services);
                }
            }

            public abstract partial class WithConsumer<TRequest, TConsumer> : IFeatureSlice.Consumer.Flag<TFeatureFlag, TRequest, TConsumer>
                where TConsumer : class, IConsumer<TRequest>
            {
                public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
                {
                    WithEndpoint<TEndpoint>.Register(hostExtender);
                    RegisterInternal(services);
                }
            }
        }
    }
}
