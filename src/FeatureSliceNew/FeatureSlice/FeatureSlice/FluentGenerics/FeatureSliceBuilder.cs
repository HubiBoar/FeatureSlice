using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice.FluentGenerics;

public static class FeatureSlice
{
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
    }

    public static class WithEndpoint<TEndpoint>
        where TEndpoint : IEndpoint
    {
        public abstract class Build<TSelf> : EndpointFeatureSlice.Default<TEndpoint>
            where TSelf : Build<TSelf>
        {
            public static void Register(HostExtender<WebApplication> hostExtender)
            {
                EndpointFeatureSlice.Default<TEndpoint>.Register(hostExtender);
            }
        }

        public static class WithHandler<TRequest, TResponse, THandler>
            where THandler : class, IHandler<TRequest, TResponse>
        {
            public abstract class Build<TSelf> : HandlerFeatureSlice.Default<TRequest, TResponse, THandler>, EndpointFeatureSlice.Default<TEndpoint>
                where TSelf : Build<TSelf>
            {
                public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
                {
                    EndpointFeatureSlice.Default<TEndpoint>.Register(hostExtender);
                    RegisterBase(services);
                }
            }
        }

        public static class WithConsumer<TRequest, TConsumer>
            where TConsumer : class, IConsumer<TRequest>
        {
            public abstract class Build<TSelf> : ConsumerFeatureSlice.Default<TRequest, TConsumer>, EndpointFeatureSlice.Default<TEndpoint>
                where TSelf : Build<TSelf>
            {
                public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
                {
                    EndpointFeatureSlice.Default<TEndpoint>.Register(hostExtender);
                    RegisterBase(services);
                }
            }
        }
    }

    public static class AsFlag
    {
        public static class WithHandler<TRequest, TResponse, THandler>
            where THandler : class, IHandler<TRequest, TResponse>
        {
            public abstract class Build<TSelf> : HandlerFeatureSlice.Flag<TSelf, TRequest, TResponse, THandler>
                where TSelf : Build<TSelf>, IFeatureFlag
            {
                public static void Register(IServiceCollection services)
                {
                    RegisterBase(services);
                }
            }
        }

        public static class WithConsumer<TRequest, TConsumer>
            where TConsumer : class, IConsumer<TRequest>
        {
            public abstract class Build<TSelf> : ConsumerFeatureSlice.Flag<TSelf, TRequest, TConsumer>
                where TSelf : Build<TSelf>, IFeatureFlag
            {
                public static void Register(IServiceCollection services)
                {
                   RegisterBase(services);
                }
            }
        }

        public static class WithEndpoint<TEndpoint>
            where TEndpoint : IEndpoint
        {
            public abstract class Build<TSelf> : EndpointFeatureSlice.Flag<TSelf, TEndpoint>
                where TSelf : Build<TSelf>, IFeatureFlag
            {
                public static void Register(HostExtender<WebApplication> hostExtender)
                {
                    EndpointFeatureSlice.Flag<TSelf, TEndpoint>.Register(hostExtender);
                }
            }

            public static class WithHandler<TRequest, TResponse, THandler>
                where THandler : class, IHandler<TRequest, TResponse>
            {
                public abstract class Build<TSelf> : HandlerFeatureSlice.Flag<TSelf, TRequest, TResponse, THandler>, EndpointFeatureSlice.Flag<TSelf, TEndpoint>
                    where TSelf : Build<TSelf>, IFeatureFlag
                {
                    public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
                    {
                        EndpointFeatureSlice.Flag<TSelf, TEndpoint>.Register(hostExtender);
                        RegisterBase(services);
                    }
                }
            }

            public static class WithConsumer<TRequest, TConsumer>
                where TConsumer : class, IConsumer<TRequest>
            {
                public abstract class Build<TSelf> : ConsumerFeatureSlice.Flag<TSelf, TRequest, TConsumer>, EndpointFeatureSlice.Flag<TSelf, TEndpoint>
                    where TSelf : Build<TSelf>, IFeatureFlag
                {
                    public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
                    {
                        EndpointFeatureSlice.Flag<TSelf, TEndpoint>.Register(hostExtender);
                        RegisterBase(services);
                    }
                }
            }

            public static class AsHandler<TRequest, TResponse>
            {
                public abstract class Build<TSelf> : HandlerFeatureSlice.Flag<TSelf, TRequest, TResponse, TSelf>, EndpointFeatureSlice.Default<TEndpoint>
                    where TSelf : Build<TSelf>, IFeatureFlag, IHandler<TRequest, TResponse>
                {
                    public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
                    {
                        EndpointFeatureSlice.Default<TEndpoint>.Register(hostExtender);
                        RegisterBase(services);
                    }
                }
            }

            public static class AsConsumer<TRequest>
            {
                public abstract class Build<TSelf> : ConsumerFeatureSlice.Flag<TSelf, TRequest, TSelf>, EndpointFeatureSlice.Default<TEndpoint>
                    where TSelf : Build<TSelf>, IFeatureFlag, IConsumer<TRequest>
                {
                    public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
                    {
                        EndpointFeatureSlice.Default<TEndpoint>.Register(hostExtender);
                        RegisterBase(services);
                    }
                }
            }
        }
        
        public static class AsEndpoint
        {
            public abstract class Build<TSelf> : EndpointFeatureSlice.Flag<TSelf, TSelf>
                where TSelf : Build<TSelf>, IEndpoint, IFeatureFlag
            {
                public static void Register(HostExtender<WebApplication> hostExtender)
                {
                    EndpointFeatureSlice.Flag<TSelf, TSelf>.Register(hostExtender);
                }
            }

            public static class WithHandler<TRequest, TResponse, THandler>
                where THandler : class, IHandler<TRequest, TResponse>
            {
                public abstract class Build<TSelf> : HandlerFeatureSlice.Flag<TSelf, TRequest, TResponse, THandler>, EndpointFeatureSlice.Flag<TSelf, TSelf>
                    where TSelf : Build<TSelf>, IEndpoint, IFeatureFlag
                {
                    public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
                    {
                        EndpointFeatureSlice.Flag<TSelf, TSelf>.Register(hostExtender);
                        RegisterBase(services);
                    }
                }
            }

            public static class WithConsumer<TRequest, TConsumer>
                where TConsumer : class, IConsumer<TRequest>
            {
                public abstract class Build<TSelf> : ConsumerFeatureSlice.Flag<TSelf, TRequest, TConsumer>, EndpointFeatureSlice.Flag<TSelf, TSelf>
                    where TSelf : Build<TSelf>, IEndpoint, IFeatureFlag
                {
                    public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
                    {
                        EndpointFeatureSlice.Flag<TSelf, TSelf>.Register(hostExtender);
                        RegisterBase(services);
                    }
                }
            }

            public static class AsHandler<TRequest, TResponse>
            {
                public abstract class Build<TSelf> : HandlerFeatureSlice.Flag<TSelf, TRequest, TResponse, TSelf>, EndpointFeatureSlice.Flag<TSelf, TSelf>
                    where TSelf : Build<TSelf>, IEndpoint, IHandler<TRequest, TResponse>, IFeatureFlag
                {
                    public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
                    {
                        EndpointFeatureSlice.Flag<TSelf, TSelf>.Register(hostExtender);
                        RegisterBase(services);
                    }
                }
            }

            public static class AsConsumer<TRequest>
            {
                public abstract class Build<TSelf> : ConsumerFeatureSlice.Flag<TSelf, TRequest, TSelf>, EndpointFeatureSlice.Flag<TSelf, TSelf>
                    where TSelf : Build<TSelf>, IEndpoint, IConsumer<TRequest>, IFeatureFlag
                {
                    public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
                    {
                        EndpointFeatureSlice.Flag<TSelf, TSelf>.Register(hostExtender);
                        RegisterBase(services);
                    }
                }
            }
        }
    }

    
    public static class AsHandler<TRequest, TResponse>
    {
        public abstract class Build<TSelf> : HandlerFeatureSlice.Default<TRequest, TResponse, TSelf>
            where TSelf : Build<TSelf>, IHandler<TRequest, TResponse>
        {
            public static void Register(IServiceCollection services)
            {
                RegisterBase(services);
            }
        }
    }

    public static class AsConsumer<TRequest>
    {
        public abstract class Build<TSelf> : ConsumerFeatureSlice.Default<TRequest, TSelf>
            where TSelf : Build<TSelf>, IConsumer<TRequest>
        {
            public static void Register(IServiceCollection services)
            {
                RegisterBase(services);
            }
        }
    }

    public static class AsEndpoint
    {
        public abstract class Build<TSelf> : EndpointFeatureSlice.Default<TSelf>
            where TSelf : Build<TSelf>, IEndpoint
        {
            public static void Register(HostExtender<WebApplication> hostExtender)
            {
                EndpointFeatureSlice.Default<TSelf>.Register(hostExtender);
            }
        }

        public static class WithHandler<TRequest, TResponse, THandler>
            where THandler : class, IHandler<TRequest, TResponse>
        {
            public abstract class Build<TSelf> : HandlerFeatureSlice.Default<TRequest, TResponse, THandler>, EndpointFeatureSlice.Default<TSelf>
                where TSelf : Build<TSelf>, IEndpoint
            {
                public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
                {
                    EndpointFeatureSlice.Default<TSelf>.Register(hostExtender);
                    RegisterBase(services);
                }
            }
        }

        public static class WithConsumer<TRequest, TConsumer>
            where TConsumer : class, IConsumer<TRequest>
        {
            public abstract class Build<TSelf> : ConsumerFeatureSlice.Default<TRequest, TConsumer>, EndpointFeatureSlice.Default<TSelf>
                where TSelf : Build<TSelf>, IEndpoint
            {
                public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
                {
                    EndpointFeatureSlice.Default<TSelf>.Register(hostExtender);
                    RegisterBase(services);
                }
            }
        }

        public static class AsHandler<TRequest, TResponse>
        {
            public abstract class Build<TSelf> : HandlerFeatureSlice.Default<TRequest, TResponse, TSelf>, EndpointFeatureSlice.Default<TSelf>
                where TSelf : Build<TSelf>, IEndpoint, IHandler<TRequest, TResponse>
            {
                public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
                {
                    EndpointFeatureSlice.Default<TSelf>.Register(hostExtender);
                    RegisterBase(services);
                }
            }
        }

        public static class AsConsumer<TRequest>
        {
            public abstract class Build<TSelf> : ConsumerFeatureSlice.Default<TRequest, TSelf>, EndpointFeatureSlice.Default<TSelf>
                where TSelf : Build<TSelf>, IEndpoint, IConsumer<TRequest>
            {
                public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
                {
                    EndpointFeatureSlice.Default<TSelf>.Register(hostExtender);
                    RegisterBase(services);
                }
            }
        }
    }
}