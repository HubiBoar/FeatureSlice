using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using FeatureSlice;

namespace FeatureSliceApproach;

public struct Disabled;

public interface IHandler<TRequest, TResponse>
{
    public Task<OneOf<TRequest, Error>> Handle(TResponse response);
}

public partial interface IForwardFacingFeatureSlice<TRequest, TResponse>
{
    public static class Delegate
    {

        public abstract partial class Default : IForwardFacingFeatureSlice<TRequest, TResponse>
        {
            public delegate Task<OneOf<TRequest, Error>> Dispatch(TResponse request);

            protected static void RegisterInternal(IServiceCollection services, Func<IServiceProvider, Dispatch> dispatcher)
            {
                services.AddSingleton<Dispatch>(provider => dispatcher(provider));
            }
        }

        public abstract partial class Toggle : IForwardFacingFeatureSlice<TRequest, TResponse>
        {
            public delegate Task<OneOf<TRequest, Disabled, Error>> Dispatch(TResponse request);

            public abstract string Name { get; }

            protected static void RegisterInternal(IServiceCollection services, Func<IServiceProvider, Dispatch> dispatcher)
            {
                services.AddSingleton<Dispatch>(provider => dispatcher(provider));
            }
        }
    }

    public static class Handler
    {
        public abstract partial class Default<THandler> : Delegate.Default
            where THandler : class, IHandler<TRequest, TResponse>
        {
            protected static void RegisterInternal(IServiceCollection services)
            {
                services.AddSingleton<THandler>();
                RegisterInternal(services, provider => provider.GetRequiredService<THandler>().Handle);
            }
        }

        public abstract partial class Toggle<THandler> : Delegate.Toggle
            where THandler : class, IHandler<TRequest, TResponse>
        {
            protected static void RegisterInternal(IServiceCollection services)
            {
                services.AddSingleton<THandler>();
                //Check if is enabled etc
                //Register(services, provider => provider.GetRequiredService<THandler>().Handle);
            }
        }
    }

    public static class Endpoint
    {
        public abstract partial class Default<TEndpoint> : IForwardFacingFeatureSlice<TRequest, TResponse>
            where TEndpoint : Feature.IEndpoint
        {
            protected static void RegisterInternal(HostExtender<WebApplication> hostExtender)
            {
                hostExtender.Map<TEndpoint>();
            }
        }

        public abstract partial class Toggle<TEndpoint> : IForwardFacingFeatureSlice<TRequest, TResponse>
            where TEndpoint : Feature.IEndpoint
        {
            public abstract string Name { get; }

            protected static void RegisterInternal(HostExtender<WebApplication> hostExtender)
            {
                hostExtender.Map<TEndpoint>();
            }
        }
    }
}

public static partial class FeatureSlice<TSelf, TRequest, TResponse>
    where TSelf : IForwardFacingFeatureSlice<TRequest, TResponse>
{
    public static partial class WithToggle
    {
        public abstract partial class WithEndpoint<TEndpoint> : IForwardFacingFeatureSlice<TRequest, TResponse>.Endpoint.Toggle<TEndpoint>
            where TEndpoint : Feature.IEndpoint
        {
            public static void Register(HostExtender<WebApplication> hostExtender)
            {
                RegisterInternal(hostExtender);
            }

            public abstract partial class WithHandler<THandler> : IForwardFacingFeatureSlice<TRequest, TResponse>.Handler.Toggle<THandler>
                where THandler : class, IHandler<TRequest, TResponse>
            {
                public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
                {
                    WithEndpoint<TEndpoint>.Register(hostExtender);
                    RegisterInternal(services);
                }
            }
        }

        public abstract partial class WithHandler<THandler> : IForwardFacingFeatureSlice<TRequest, TResponse>.Handler.Toggle<THandler>
            where THandler : class, IHandler<TRequest, TResponse>
        {
            public static void Register(IServiceCollection services)
            {
                RegisterInternal(services);
            }
        }
    }

    public abstract partial class WithEndpoint<TEndpoint> :  IForwardFacingFeatureSlice<TRequest, TResponse>.Endpoint.Default<TEndpoint>
        where TEndpoint : Feature.IEndpoint
    {
        public static void Register(HostExtender<WebApplication> hostExtender)
        {
            RegisterInternal(hostExtender);
        }
    }

    public abstract partial class WithHandler<THandler> : IForwardFacingFeatureSlice<TRequest, TResponse>.Handler.Default<THandler>
        where THandler : class, IHandler<TRequest, TResponse>
    {
        public static void Register(IServiceCollection services)
        {
            RegisterInternal(services);
        }
    }
}

public sealed class ExampleFeature : FeatureSlice<
    ExampleFeature,
    ExampleFeature.Request,
    ExampleFeature.Response>
    .WithToggle
    .WithEndpoint<ExampleFeature.Endpoint>
    .WithHandler<ExampleFeature.Handler>
{
    public record Request();
    public record Response();

    public override string Name => "ExampleFeature";

    public class Handler : IHandler<Request, Response>
    {
        public Task<OneOf<Request, Error>> Handle(Response response)
        {
            throw new NotImplementedException();
        }
    }

    public class Endpoint : Feature.IEndpoint
    {
        public static Feature.EndpointSetup Setup =>
            Feature.EndpointSetup.MapPut("test", () => {});
    }
}

public class Usage
{
    public static void Use(ExampleFeature.Dispatch dispatch)
    {
        dispatch.Invoke(new ExampleFeature.Response());
    }

    public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
    {
        ExampleFeature.Register(services, hostExtender);
    }
}

