using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

public interface IFeatureSlice
{
}

public static partial class FeatureSlice
{
    public static class AsEndpoint
    {
        public abstract class Build<TSelf> : EndpointFeatureSlice.Default<TSelf>
            where TSelf : Build<TSelf>, IEndpoint
        {
            public static void RegisterBase(HostExtender<WebApplication> hostExtender)
            {
                EndpointFeatureSlice.Default<TSelf>.RegisterBase(hostExtender);
            }
        }

        public static class AsFlag
        {
            public abstract class Build<TSelf> : EndpointFeatureSlice.Flag<TSelf, TSelf>
                where TSelf : Build<TSelf>, IEndpoint, IFeatureFlag
            {
                public static void RegisterBase(HostExtender<WebApplication> hostExtender)
                {
                    EndpointFeatureSlice.Flag<TSelf, TSelf>.RegisterBase(hostExtender);
                }
            }
        }
    }

    public static class AsHandler<TRequest, TResponse>
    {
        public abstract class Build<TSelf> : StaticHandlerFeatureSlice.Default<TRequest, TResponse, TSelf>
            where TSelf : Build<TSelf>, IStaticHandler<TRequest, TResponse>
        {
            public static void Register(IServiceCollection services)
            {
                RegisterBase(services);
            }
        }

        public static class AsEndpoint
        {
            public abstract class Build<TSelf> : StaticHandlerFeatureSlice.Default<TRequest, TResponse, TSelf>, EndpointFeatureSlice.Default<TSelf>
                where TSelf : Build<TSelf>, IEndpoint, IStaticHandler<TRequest, TResponse>
            {
                public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
                {
                    EndpointFeatureSlice.Default<TSelf>.RegisterBase(hostExtender);
                    RegisterBase(services);
                }
            }

            public static class AsFlag
            {
                public abstract class Build<TSelf> : StaticHandlerFeatureSlice.Flag<TSelf, TRequest, TResponse, TSelf>, EndpointFeatureSlice.Flag<TSelf, TSelf>
                    where TSelf : Build<TSelf>, IEndpoint, IFeatureFlag, IStaticHandler<TRequest, TResponse>
                {
                    public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
                    {
                        EndpointFeatureSlice.Flag<TSelf, TSelf>.RegisterBase(hostExtender);
                        RegisterBase(services);
                    }
                }
            }
        }
    }

    public static class WithHandler<TRequest, TResponse, THandler>
        where THandler : class, IHandler<TRequest, TResponse>
    {
        public abstract class Build<TSelf> : HandlerFeatureSlice.Default<TRequest, TResponse, THandler>
            where TSelf : Build<TSelf>
        {
            public static void Register(IServiceCollection services)
            {
                RegisterBase(services);
            }
        }

        public static class AsEndpoint
        {
            public abstract class Build<TSelf> : HandlerFeatureSlice.Default<TRequest, TResponse, THandler>, EndpointFeatureSlice.Default<TSelf>
                where TSelf : Build<TSelf>, IEndpoint
            {
                public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
                {
                    EndpointFeatureSlice.Default<TSelf>.RegisterBase(hostExtender);
                    RegisterBase(services);
                }
            }

            public static class AsFlag
            {
                public abstract class Build<TSelf> : HandlerFeatureSlice.Flag<TSelf, TRequest, TResponse, THandler>, EndpointFeatureSlice.Flag<TSelf, TSelf>
                    where TSelf : Build<TSelf>, IEndpoint, IFeatureFlag
                {
                    public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
                    {
                        EndpointFeatureSlice.Flag<TSelf, TSelf>.RegisterBase(hostExtender);
                        RegisterBase(services);
                    }
                }
            }
        }
    }

    public static class WithConsumer<TRequest, TConsumer>
        where TConsumer : class, IConsumer<TRequest>
    {
        public abstract class Build<TSelf> : ConsumerFeatureSlice.Default<TRequest, TConsumer>
            where TSelf : Build<TSelf>
        {
            public static void Register(IServiceCollection services)
            {
                RegisterBase(services);
            }
        }

        public static class AsEndpoint
        {
            public abstract class Build<TSelf> : ConsumerFeatureSlice.Default<TRequest, TConsumer>, EndpointFeatureSlice.Default<TSelf>
                where TSelf : Build<TSelf>, IEndpoint
            {
                public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
                {
                    EndpointFeatureSlice.Default<TSelf>.RegisterBase(hostExtender);
                    RegisterBase(services);
                }
            }

            public static class AsFlag
            {
                public abstract class Build<TSelf> : ConsumerFeatureSlice.Flag<TSelf, TRequest, TConsumer>, EndpointFeatureSlice.Flag<TSelf, TSelf>
                    where TSelf : Build<TSelf>, IEndpoint, IFeatureFlag
                {
                    public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
                    {
                        EndpointFeatureSlice.Flag<TSelf, TSelf>.RegisterBase(hostExtender);
                        RegisterBase(services);
                    }
                }
            }
        }
    }
}