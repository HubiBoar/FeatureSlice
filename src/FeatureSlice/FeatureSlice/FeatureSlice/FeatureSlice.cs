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
        public abstract class BuildAs<TSelf> : EndpointFeatureSlice.Default<TSelf>
            where TSelf : BuildAs<TSelf>, IEndpoint
        {
            public static void Register(HostExtender<WebApplication> hostExtender)
            {
                EndpointFeatureSlice.Default<TSelf>.RegisterBase(hostExtender);
            }
        }

        public abstract class Build<TSelf> : BuildAs<TSelf>, IEndpoint
            where TSelf : Build<TSelf>, new()
        {
            protected abstract IEndpoint.Setup Endpoint { get; }

            static IEndpoint.Setup IEndpoint.Endpoint => new TSelf().Endpoint;
        }

        public static class AsFlag
        {
            public abstract class BuildAs<TSelf> : EndpointFeatureSlice.Flag<TSelf, TSelf>
                where TSelf : BuildAs<TSelf>, IEndpoint, IFeatureFlag
            {
                public static void Register(HostExtender<WebApplication> hostExtender)
                {
                    EndpointFeatureSlice.Flag<TSelf, TSelf>.RegisterBase(hostExtender);
                }
            }

            public abstract class Build<TSelf> : BuildAs<TSelf>, IEndpoint, IFeatureFlag
                where TSelf : Build<TSelf>, new()
            {
                protected abstract IEndpoint.Setup Endpoint { get; }
                protected abstract string FeatureName { get; }

                static string IFeatureFlag.FeatureName => new TSelf().FeatureName;
                static IEndpoint.Setup IEndpoint.Endpoint => new TSelf().Endpoint;
            }
        }
    }

    public static class AsHandler<TRequest, TResponse, TDependencies>
        where TDependencies : class, IFromServices<TDependencies>
    {
        public abstract class BuildAs<TSelf> : StaticHandlerFeatureSlice.Default<TRequest, TResponse, TSelf, TDependencies>
            where TSelf : BuildAs<TSelf>, IStaticHandler<TRequest, TResponse, TDependencies>
        {
            public static void Register(IServiceCollection services)
            {
                RegisterBase(services);
            }
        }

        public abstract class Build<TSelf> : BuildAs<TSelf>, IStaticHandler<TRequest, TResponse, TDependencies>
            where TSelf : Build<TSelf>, new ()
        {
            protected abstract Task<OneOf<TResponse, Error>> Handle(TRequest request, TDependencies dependencies);

            static Task<OneOf<TResponse, Error>> IStaticHandler<TRequest, TResponse, TDependencies>.Handle(TRequest request, TDependencies dependencies) => new TSelf().Handle(request, dependencies);
        }

        public static class AsEndpoint
        {
            public abstract class BuildAs<TSelf> : StaticHandlerFeatureSlice.Default<TRequest, TResponse, TSelf, TDependencies>, EndpointFeatureSlice.Default<TSelf>
                where TSelf : BuildAs<TSelf>, IEndpoint, IStaticHandler<TRequest, TResponse, TDependencies>
            {
                public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
                {
                    EndpointFeatureSlice.Default<TSelf>.RegisterBase(hostExtender);
                    RegisterBase(services);
                }
            }

            public abstract class Build<TSelf> : BuildAs<TSelf>, IEndpoint, IStaticHandler<TRequest, TResponse, TDependencies>
                where TSelf : Build<TSelf>, new ()
            {
                protected abstract IEndpoint.Setup Endpoint { get; }
                protected abstract Task<OneOf<TResponse, Error>> Handle(TRequest request, TDependencies dependencies);

                static IEndpoint.Setup IEndpoint.Endpoint => new TSelf().Endpoint;
                static Task<OneOf<TResponse, Error>> IStaticHandler<TRequest, TResponse, TDependencies>.Handle(TRequest request, TDependencies dependencies) => new TSelf().Handle(request, dependencies);
            }

            public static class AsFlag
            {
                public abstract class BuildAs<TSelf> : StaticHandlerFeatureSlice.Flag<TSelf, TRequest, TResponse, TSelf, TDependencies>, EndpointFeatureSlice.Flag<TSelf, TSelf>
                    where TSelf : BuildAs<TSelf>, IEndpoint, IFeatureFlag, IStaticHandler<TRequest, TResponse, TDependencies>
                {
                    public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
                    {
                        EndpointFeatureSlice.Flag<TSelf, TSelf>.RegisterBase(hostExtender);
                        RegisterBase(services);
                    }
                }

                public abstract class Build<TSelf> : BuildAs<TSelf>, IEndpoint, IFeatureFlag, IStaticHandler<TRequest, TResponse, TDependencies>
                    where TSelf : Build<TSelf>, new()
                {
                    protected abstract IEndpoint.Setup Endpoint { get; }
                    protected abstract string FeatureName { get; }
                    protected abstract Task<OneOf<TResponse, Error>> Handle(TRequest request, TDependencies dependencies);

                    static IEndpoint.Setup IEndpoint.Endpoint => new TSelf().Endpoint;
                    static string IFeatureFlag.FeatureName => new TSelf().FeatureName;
                    static Task<OneOf<TResponse, Error>> IStaticHandler<TRequest, TResponse, TDependencies>.Handle(TRequest request, TDependencies dependencies) => new TSelf().Handle(request, dependencies);
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
            public abstract class BuildAs<TSelf> : HandlerFeatureSlice.Default<TRequest, TResponse, THandler>, EndpointFeatureSlice.Default<TSelf>
                where TSelf : BuildAs<TSelf>, IEndpoint
            {
                public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
                {
                    EndpointFeatureSlice.Default<TSelf>.RegisterBase(hostExtender);
                    RegisterBase(services);
                }
            }

            public abstract class Build<TSelf> : BuildAs<TSelf>, IEndpoint
                where TSelf : Build<TSelf>, new ()
            {
                protected abstract IEndpoint.Setup Endpoint { get; }

                static IEndpoint.Setup IEndpoint.Endpoint => new TSelf().Endpoint;
            }

            public static class AsFlag
            {
                public abstract class BuildAs<TSelf> : HandlerFeatureSlice.Flag<TSelf, TRequest, TResponse, THandler>, EndpointFeatureSlice.Flag<TSelf, TSelf>
                    where TSelf : BuildAs<TSelf>, IEndpoint, IFeatureFlag
                {
                    public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
                    {
                        EndpointFeatureSlice.Flag<TSelf, TSelf>.RegisterBase(hostExtender);
                        RegisterBase(services);
                    }
                }

                public abstract class Build<TSelf> : BuildAs<TSelf>, IEndpoint, IFeatureFlag
                    where TSelf : Build<TSelf>, new ()
                {
                    protected abstract IEndpoint.Setup Endpoint { get; }
                    protected abstract string FeatureName { get; }

                    static IEndpoint.Setup IEndpoint.Endpoint => new TSelf().Endpoint;
                    static string IFeatureFlag.FeatureName => new TSelf().FeatureName;
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
            public static void Register(IServiceCollection services, Messaging.ISetupProvider setupProvider)
            {
                RegisterBase(services, setupProvider.GetSetup);
            }
        }

        public static class AsEndpoint
        {
            public abstract class BuildAs<TSelf> : ConsumerFeatureSlice.Default<TRequest, TConsumer>, EndpointFeatureSlice.Default<TSelf>
                where TSelf : BuildAs<TSelf>, IEndpoint
            {
                public static void Register(IServiceCollection services, Messaging.ISetupProvider setupProvider, HostExtender<WebApplication> hostExtender)
                {
                    EndpointFeatureSlice.Default<TSelf>.RegisterBase(hostExtender);
                    RegisterBase(services, setupProvider.GetSetup);
                }
            }

            public abstract class Build<TSelf> : BuildAs<TSelf>, IEndpoint
                where TSelf : Build<TSelf>, new()
            {
                protected abstract IEndpoint.Setup Endpoint { get; }

                static IEndpoint.Setup IEndpoint.Endpoint => new TSelf().Endpoint;
            }

            public static class AsFlag
            {
                public abstract class BuildAs<TSelf> : ConsumerFeatureSlice.Flag<TSelf, TRequest, TConsumer>, EndpointFeatureSlice.Flag<TSelf, TSelf>
                    where TSelf : BuildAs<TSelf>, IEndpoint, IFeatureFlag
                {
                    public static void Register(IServiceCollection services, Messaging.ISetupProvider setupProvider, HostExtender<WebApplication> hostExtender)
                    {
                        EndpointFeatureSlice.Flag<TSelf, TSelf>.RegisterBase(hostExtender);
                        RegisterBase(services, setupProvider.GetSetup);
                    }
                }

                public abstract class Build<TSelf> : BuildAs<TSelf>, IEndpoint, IFeatureFlag
                    where TSelf : Build<TSelf>, new()
                {
                    protected abstract IEndpoint.Setup Endpoint { get; }
                    protected abstract string FeatureName { get; }

                    static IEndpoint.Setup IEndpoint.Endpoint => new TSelf().Endpoint;
                    static string IFeatureFlag.FeatureName => new TSelf().FeatureName;
                }
            }
        }
    }

    public static class AsConsumer<TRequest, TDependencies>
        where TDependencies : class, IFromServices<TDependencies>
    {
        public abstract class BuildAs<TSelf> : StaticConsumerFeatureSlice.Default<TRequest, TSelf, TDependencies>
            where TSelf : BuildAs<TSelf>, IStaticConsumer<TRequest, TDependencies>
        {
            public static void Register(IServiceCollection services, Messaging.ISetupProvider setupProvider)
            {
                RegisterBase(services, setupProvider.GetSetup);
            }
        }

        public abstract class Build<TSelf> : BuildAs<TSelf>, IStaticConsumer<TRequest, TDependencies>
            where TSelf : Build<TSelf>, new()
        {
            protected abstract ConsumerName ConsumerName { get; }
            protected abstract Task<OneOf<Success, Error>> Consume(TRequest request, TDependencies dependencies);

            static ConsumerName IStaticConsumer<TRequest, TDependencies>.ConsumerName => new TSelf().ConsumerName;
            static Task<OneOf<Success, Error>> IStaticConsumer<TRequest, TDependencies>.Consume(TRequest request, TDependencies dependencies) => new TSelf().Consume(request, dependencies);
        }

        public static class AsEndpoint
        {
            public abstract class BuildAs<TSelf> : StaticConsumerFeatureSlice.Default<TRequest, TSelf, TDependencies>, EndpointFeatureSlice.Default<TSelf>
                where TSelf : BuildAs<TSelf>, IEndpoint, IStaticConsumer<TRequest, TDependencies>
            {
                public static void Register(IServiceCollection services, Messaging.ISetupProvider setupProvider, HostExtender<WebApplication> hostExtender)
                {
                    EndpointFeatureSlice.Default<TSelf>.RegisterBase(hostExtender);
                    RegisterBase(services, setupProvider.GetSetup);
                }
            }

            public abstract class Build<TSelf> : BuildAs<TSelf>, IEndpoint, IStaticConsumer<TRequest, TDependencies>
                where TSelf : Build<TSelf>, new()
            {
                protected abstract IEndpoint.Setup Endpoint { get; }
                protected abstract ConsumerName ConsumerName { get; }
                protected abstract Task<OneOf<Success, Error>> Consume(TRequest request, TDependencies dependencies);

                static IEndpoint.Setup IEndpoint.Endpoint => new TSelf().Endpoint;
                static ConsumerName IStaticConsumer<TRequest, TDependencies>.ConsumerName => new TSelf().ConsumerName;
                static Task<OneOf<Success, Error>> IStaticConsumer<TRequest, TDependencies>.Consume(TRequest request, TDependencies dependencies) => new TSelf().Consume(request, dependencies);
            }

            public static class AsFlag
            {
                public abstract class BuildAs<TSelf> : StaticConsumerFeatureSlice.Flag<TSelf, TRequest, TSelf, TDependencies>, EndpointFeatureSlice.Flag<TSelf, TSelf>
                    where TSelf : BuildAs<TSelf>, IEndpoint, IFeatureFlag, IStaticConsumer<TRequest, TDependencies>
                {
                    public static void Register(IServiceCollection services, Messaging.ISetupProvider setupProvider, HostExtender<WebApplication> hostExtender)
                    {
                        EndpointFeatureSlice.Flag<TSelf, TSelf>.RegisterBase(hostExtender);
                        RegisterBase(services, setupProvider.GetSetup);
                    }
                }

                public abstract class Build<TSelf> : BuildAs<TSelf>, IEndpoint, IFeatureFlag, IStaticConsumer<TRequest, TDependencies>
                    where TSelf : Build<TSelf>, new()
                {
                    protected abstract string FeatureName { get; }
                    protected abstract IEndpoint.Setup Endpoint { get; }
                    protected abstract ConsumerName ConsumerName { get; }
                    protected abstract Task<OneOf<Success, Error>> Consume(TRequest request, TDependencies dependencies);

                    static IEndpoint.Setup IEndpoint.Endpoint => new TSelf().Endpoint;
                    static string IFeatureFlag.FeatureName => new TSelf().FeatureName;
                    static ConsumerName IStaticConsumer<TRequest, TDependencies>.ConsumerName => new TSelf().ConsumerName;
                    static Task<OneOf<Success, Error>> IStaticConsumer<TRequest, TDependencies>.Consume(TRequest request, TDependencies dependencies) => new TSelf().Consume(request, dependencies);
                }
            }
        }
    }
}