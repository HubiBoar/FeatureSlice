using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;

namespace FeatureSlice.FluentGenerics.Interfaces;

public partial interface IFeatureSlice
{   
}


public static partial class FeatureSlice
{
    public interface WithHandler<TRequest, TResponse, THandler> : HandlerFeatureSlice.Default<TRequest, TResponse, THandler>
        where THandler : class, IHandler<TRequest, TResponse>
    {
        public static void Register(IServiceCollection services)
        {
            RegisterInternal(services);
        }
    }

    public interface WithConsumer<TRequest, TConsumer> : ConsumerFeatureSlice.Default<TRequest, TConsumer>
        where TConsumer : class, IConsumer<TRequest>
    {
        public static void Register(IServiceCollection services)
        {
            RegisterInternal(services);
        }
    }

    public interface WithEndpoint<TEndpoint> : EndpointFeatureSlice.Default<TEndpoint>
        where TEndpoint : IEndpoint
    {
        public static void Register(HostExtender<WebApplication> hostExtender)
        {
            RegisterInternal(hostExtender);
        }

        public interface WithHandler<TRequest, TResponse, THandler> : HandlerFeatureSlice.Default<TRequest, TResponse, THandler>
            where THandler : class, IHandler<TRequest, TResponse>
        {
            public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
            {
                WithEndpoint<TEndpoint>.Register(hostExtender);
                RegisterInternal(services);
            }
        }

        public interface WithConsumer<TRequest, TConsumer> : ConsumerFeatureSlice.Default<TRequest, TConsumer>
            where TConsumer : class, IConsumer<TRequest>
        {
            public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
            {
                WithEndpoint<TEndpoint>.Register(hostExtender);
                RegisterInternal(services);
            }
        }
    }

    public interface WithFlag : IFeatureFlag
    {
        public interface WithHandler<TRequest, TResponse, THandler> : HandlerFeatureSlice.Flag<TRequest, TResponse, THandler>
            where THandler : class, IHandler<TRequest, TResponse>
        {
            public static void Register(IServiceCollection services)
            {
                RegisterInternal(services);
            }
        }

        public interface WithConsumer<TRequest, TConsumer> : ConsumerFeatureSlice.Flag<TRequest, TConsumer>
            where TConsumer : class, IConsumer<TRequest>
        {
            public static void Register(IServiceCollection services)
            {
                RegisterInternal(services);
            }
        }

        public interface AsEndpoint : EndpointFeatureSlice.AsFlag
        {
            public abstract static void Register(HostExtender<WebApplication> hostExtender);
        }

        public interface WithEndpoint<TEndpoint> : EndpointFeatureSlice.Flag<TEndpoint>
            where TEndpoint : IEndpoint
        {
            public abstract static void Register(HostExtender<WebApplication> hostExtender);

            public interface WithHandler<TRequest, TResponse, THandler> : HandlerFeatureSlice.Flag<TRequest, TResponse, THandler>, EndpointFeatureSlice.Flag<TEndpoint>
                where THandler : class, IHandler<TRequest, TResponse>
            {
                public abstract static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender);

                protected static void RegisterInternal(IServiceCollection services, HostExtender<WebApplication> hostExtender)
                {
                    RegisterInternal(hostExtender);
                    RegisterInternal(services);
                }
            }

            public interface WithConsumer<TRequest, TConsumer> : ConsumerFeatureSlice.Flag<TRequest, TConsumer>, EndpointFeatureSlice.Flag<TEndpoint>
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