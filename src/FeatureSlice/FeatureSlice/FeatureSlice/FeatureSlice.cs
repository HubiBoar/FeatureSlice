using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;

namespace FeatureSlice;

public interface IFeatureSlice
{
}

public static partial class FeatureSlice
{
    public static class AsEndpoint
    {
        public abstract class BuildBase<TSelf> : EndpointFeatureSlice.Default<TSelf>
            where TSelf : BuildBase<TSelf>, IEndpoint
        {
            public static void Register(HostExtender<WebApplication> hostExtender)
            {
                EndpointFeatureSlice.Default<TSelf>.RegisterBase(hostExtender);
            }
        }

        public abstract class Build<TSelf> : BuildBase<TSelf>, IEndpoint
            where TSelf : Build<TSelf>, new()
        {
            static IEndpoint.Setup IEndpoint.Endpoint => new TSelf().Endpoint;

            protected abstract IEndpoint.Setup Endpoint { get; }
        }

        public static class AsFlag
        {
            public abstract class BuildBase<TSelf> : EndpointFeatureSlice.Flag<TSelf, TSelf>
                where TSelf : BuildBase<TSelf>, IEndpoint, IFeatureFlag
            {
                public static void Register(HostExtender<WebApplication> hostExtender)
                {
                    EndpointFeatureSlice.Flag<TSelf, TSelf>.RegisterBase(hostExtender);
                }
            }

            public abstract class Build<TSelf> : BuildBase<TSelf>, IEndpoint, IFeatureFlag
                where TSelf : Build<TSelf>, new()
            {
                static IEndpoint.Setup IEndpoint.Endpoint => new TSelf().Endpoint;

                protected abstract IEndpoint.Setup Endpoint { get; }

                static string IFeatureFlag.FeatureName => new TSelf().FeatureName;

                protected abstract string FeatureName { get; }
            }
        }
    }

    public static class AsHandler<TRequest, TResponse, TDependencies>
        where TDependencies : class, IFromServices<TDependencies>
    {
        public abstract class BuildBase<TSelf> : StaticHandlerFeatureSlice.Default<TRequest, TResponse, TSelf, TDependencies>
            where TSelf : BuildBase<TSelf>, IStaticHandler<TRequest, TResponse, TDependencies>
        {
            public static void Register(IServiceCollection services)
            {
                RegisterBase(services);
            }
        }

        public abstract class Build<TSelf> : BuildBase<TSelf>, IStaticHandler<TRequest, TResponse, TDependencies>
            where TSelf : Build<TSelf>, new ()
        {
            static Task<OneOf<TResponse, Error>> IStaticHandler<TRequest, TResponse, TDependencies>.Handle(TRequest request, TDependencies dependencies)
            {
                return new TSelf().Handle(request, dependencies);
            }

            protected abstract Task<OneOf<TResponse, Error>> Handle(TRequest request, TDependencies dependencies);
        }

        public static class AsEndpoint
        {
            public abstract class BuildBase<TSelf> : StaticHandlerFeatureSlice.Default<TRequest, TResponse, TSelf, TDependencies>, EndpointFeatureSlice.Default<TSelf>
                where TSelf : BuildBase<TSelf>, IEndpoint, IStaticHandler<TRequest, TResponse, TDependencies>
            {
                public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
                {
                    EndpointFeatureSlice.Default<TSelf>.RegisterBase(hostExtender);
                    RegisterBase(services);
                }
            }

            public abstract class Build<TSelf> : BuildBase<TSelf>, IEndpoint, IStaticHandler<TRequest, TResponse, TDependencies>
                where TSelf : Build<TSelf>, new ()
            {
                static IEndpoint.Setup IEndpoint.Endpoint => new TSelf().Endpoint;

                protected abstract IEndpoint.Setup Endpoint { get; }

                static Task<OneOf<TResponse, Error>> IStaticHandler<TRequest, TResponse, TDependencies>.Handle(TRequest request, TDependencies dependencies)
                {
                    return new TSelf().Handle(request, dependencies);
                }

                protected abstract Task<OneOf<TResponse, Error>> Handle(TRequest request, TDependencies dependencies);
            }

            public static class AsFlag
            {
                public abstract class BuildBase<TSelf> : StaticHandlerFeatureSlice.Flag<TSelf, TRequest, TResponse, TSelf, TDependencies>, EndpointFeatureSlice.Flag<TSelf, TSelf>
                    where TSelf : BuildBase<TSelf>, IEndpoint, IFeatureFlag, IStaticHandler<TRequest, TResponse, TDependencies>
                {
                    public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
                    {
                        EndpointFeatureSlice.Flag<TSelf, TSelf>.RegisterBase(hostExtender);
                        RegisterBase(services);
                    }
                }

                public abstract class Build<TSelf> : BuildBase<TSelf>, IEndpoint, IFeatureFlag, IStaticHandler<TRequest, TResponse, TDependencies>
                    where TSelf : Build<TSelf>, new()
                {
                    static IEndpoint.Setup IEndpoint.Endpoint => new TSelf().Endpoint;

                    protected abstract IEndpoint.Setup Endpoint { get; }

                    static string IFeatureFlag.FeatureName => new TSelf().FeatureName;

                    protected abstract string FeatureName { get; }

                    static Task<OneOf<TResponse, Error>> IStaticHandler<TRequest, TResponse, TDependencies>.Handle(TRequest request, TDependencies dependencies)
                    {
                        return new TSelf().Handle(request, dependencies);
                    }

                    protected abstract Task<OneOf<TResponse, Error>> Handle(TRequest request, TDependencies dependencies);
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
            public abstract class BuildBase<TSelf> : HandlerFeatureSlice.Default<TRequest, TResponse, THandler>, EndpointFeatureSlice.Default<TSelf>
                where TSelf : BuildBase<TSelf>, IEndpoint
            {
                public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
                {
                    EndpointFeatureSlice.Default<TSelf>.RegisterBase(hostExtender);
                    RegisterBase(services);
                }
            }

            public abstract class Build<TSelf> : BuildBase<TSelf>, IEndpoint
                where TSelf : Build<TSelf>, new ()
            {
                static IEndpoint.Setup IEndpoint.Endpoint => new TSelf().Endpoint;

                protected abstract IEndpoint.Setup Endpoint { get; }
            }

            public static class AsFlag
            {
                public abstract class BuildBase<TSelf> : HandlerFeatureSlice.Flag<TSelf, TRequest, TResponse, THandler>, EndpointFeatureSlice.Flag<TSelf, TSelf>
                    where TSelf : BuildBase<TSelf>, IEndpoint, IFeatureFlag
                {
                    public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
                    {
                        EndpointFeatureSlice.Flag<TSelf, TSelf>.RegisterBase(hostExtender);
                        RegisterBase(services);
                    }
                }

                public abstract class Build<TSelf> : BuildBase<TSelf>, IEndpoint, IFeatureFlag
                    where TSelf : Build<TSelf>, new ()
                {
                    static IEndpoint.Setup IEndpoint.Endpoint => new TSelf().Endpoint;

                    protected abstract IEndpoint.Setup Endpoint { get; }

                    static string IFeatureFlag.FeatureName => new TSelf().FeatureName;

                    protected abstract string FeatureName { get; }
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
            public abstract class BuildBase<TSelf> : ConsumerFeatureSlice.Default<TRequest, TConsumer>, EndpointFeatureSlice.Default<TSelf>
                where TSelf : BuildBase<TSelf>, IEndpoint
            {
                public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
                {
                    EndpointFeatureSlice.Default<TSelf>.RegisterBase(hostExtender);
                    RegisterBase(services);
                }
            }

            public abstract class Build<TSelf> : BuildBase<TSelf>, IEndpoint
                where TSelf : Build<TSelf>, new()
            {
                static IEndpoint.Setup IEndpoint.Endpoint => new TSelf().Endpoint;

                protected abstract IEndpoint.Setup Endpoint { get; }
            }

            public static class AsFlag
            {
                public abstract class BuildBase<TSelf> : ConsumerFeatureSlice.Flag<TSelf, TRequest, TConsumer>, EndpointFeatureSlice.Flag<TSelf, TSelf>
                    where TSelf : BuildBase<TSelf>, IEndpoint, IFeatureFlag
                {
                    public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
                    {
                        EndpointFeatureSlice.Flag<TSelf, TSelf>.RegisterBase(hostExtender);
                        RegisterBase(services);
                    }
                }

                public abstract class Build<TSelf> : BuildBase<TSelf>, IEndpoint, IFeatureFlag
                    where TSelf : Build<TSelf>, new()
                {
                    static IEndpoint.Setup IEndpoint.Endpoint => new TSelf().Endpoint;

                    protected abstract IEndpoint.Setup Endpoint { get; }

                    static string IFeatureFlag.FeatureName => new TSelf().FeatureName;

                    protected abstract string FeatureName { get; }
                }
            }
        }
    }
}