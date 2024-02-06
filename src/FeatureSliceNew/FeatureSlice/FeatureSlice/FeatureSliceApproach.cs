using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using FeatureSlice;
using Microsoft.AspNetCore.Http;
using Microsoft.FeatureManagement;

namespace FeatureSliceApproach;

public struct Disabled;

public interface IHandler<TRequest, TResponse> : IMethod<TRequest, Task<OneOf<TResponse, Error>>>
{
}

public interface IFeatureFlag
{
    public abstract static string FeatureName { get; }
}


public static class InMemoryDispatcher
{
    public static Task<OneOf<TResponse, Error>> Dispatch<TRequest, TResponse, THandler>(
        TRequest request,
        IServiceProvider provider)
        where THandler : IMethod<TRequest, Task<OneOf<TResponse, Error>>>
    {
        return Dispatch(
            request,
            provider.GetRequiredService<THandler>(),
            provider.GetServices<IMethod<TRequest, Task<OneOf<TResponse, Error>>>.IPipeline>().ToList());
    }

    public static async Task<OneOf<TResponse, Error>> Dispatch<TRequest, TResponse, THandler>(
        TRequest request,
        THandler self,
        IReadOnlyList<IMethod<TRequest, Task<OneOf<TResponse, Error>>>.IPipeline> pipelines)
        where THandler : IMethod<TRequest, Task<OneOf<TResponse, Error>>>
    {
        return await pipelines.RunPipeline(request, self.Handle);
    }

    public static class WithFlag<TFeatureFlag>
        where TFeatureFlag : IFeatureFlag
    {
        public static Task<OneOf<TResponse, Disabled, Error>> Dispatch<TRequest, TResponse, THandler>(
            TRequest request,
            IServiceProvider provider)
            where THandler : IMethod<TRequest, Task<OneOf<TResponse, Error>>>
        {
            return Dispatch(
                request,
                provider.GetRequiredService<THandler>(),
                provider.GetRequiredService<IFeatureManager>(),
                provider.GetServices<IMethod<TRequest, Task<OneOf<TResponse, Error>>>.IPipeline>().ToList());
        }

        public static async Task<OneOf<TResponse, Disabled, Error>> Dispatch<TRequest, TResponse, THandler>(
            TRequest request,
            THandler self,
            IFeatureManager featureManager,
            IReadOnlyList<IMethod<TRequest, Task<OneOf<TResponse, Error>>>.IPipeline> pipelines)
            where THandler : IMethod<TRequest, Task<OneOf<TResponse, Error>>>
        {
            var isEnabled = await featureManager.IsEnabledAsync(TFeatureFlag.FeatureName);
            if(isEnabled == false)
            {
                return new Disabled();
            }

            var result = await InMemoryDispatcher.Dispatch<TRequest, TResponse, THandler>(request, self, pipelines);
            return result.Match<OneOf<TResponse, Disabled, Error>>(success => success, error => error);
        }
    }
}


public abstract class EndpointHelper
{
    public static EndpointInfo MapGet(string pattern, Delegate handler) => IEndpoint.MapGet(pattern, handler);
}

public sealed record EndpointInfo(HttpMethod Method, string Pattern, Delegate Handler);

public interface IEndpoint
{
    public static abstract EndpointInfo Map { get; }

    public static EndpointInfo MapGet(string pattern, Delegate handler)
    {
        return new EndpointInfo(HttpMethod.Get, pattern, handler);
    }
}

public static class EndpointExtensions
{
    public static HostExtender<WebApplication> Map<T>(this HostExtender<WebApplication> extender)
        where T : IEndpoint
    {
        var endpointInfo = T.Map;
        extender.AddExtension(host => host.MapMethods(endpointInfo.Pattern, [ endpointInfo.Method.ToString() ], endpointInfo.Handler));

        return extender;
    }
}

public partial interface IForwardFacingFeatureSlice
{
    public static class Delegate
    {
        public abstract partial class Default<TRequest, TResponse> : IForwardFacingFeatureSlice
        {
            public delegate Task<OneOf<TResponse, Error>> Dispatch(TRequest request);

            protected static void RegisterInternal(IServiceCollection services, Func<IServiceProvider, Dispatch> dispatcher)
            {
                services.AddSingleton<Dispatch>(provider => dispatcher(provider));
            }
        }

        public abstract partial class Flag<TFeatureFlag, TRequest, TResponse> : IForwardFacingFeatureSlice
            where TFeatureFlag : IFeatureFlag
        {
            public delegate Task<OneOf<TResponse, Disabled, Error>> Dispatch(TRequest request);

            protected static void RegisterInternal(IServiceCollection services, Func<IServiceProvider, Dispatch> dispatcher)
            {
                services.AddSingleton<Dispatch>(provider => dispatcher(provider));
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

    public static class Endpoint
    {
        public abstract partial class Default<TEndpoint> : IForwardFacingFeatureSlice
            where TEndpoint : IEndpoint
        {
            protected static void RegisterInternal(HostExtender<WebApplication> hostExtender)
            {
                hostExtender.Map<TEndpoint>();
            }
        }

        public abstract partial class Flag<TFeatureFlag, TEndpoint> : IForwardFacingFeatureSlice
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

public static partial class FeatureSlice<TSelf>
    where TSelf : IForwardFacingFeatureSlice
{
    public static partial class WithFlag<TFeatureFlag>
        where TFeatureFlag : IFeatureFlag
    {
        public abstract partial class WithEndpoint<TEndpoint> : IForwardFacingFeatureSlice.Endpoint.Flag<TFeatureFlag, TEndpoint>
            where TEndpoint : IEndpoint
        {
            public static void Register(HostExtender<WebApplication> hostExtender)
            {
                RegisterInternal(hostExtender);
            }

            public abstract partial class WithHandler<TRequest, TResponse, THandler> : IForwardFacingFeatureSlice.Handler.Flag<TFeatureFlag, TRequest, TResponse, THandler>
                where THandler : class, IHandler<TRequest, TResponse>
            {
                public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
                {
                    WithEndpoint<TEndpoint>.Register(hostExtender);
                    RegisterInternal(services);
                }
            }
        }

        public abstract partial class WithHandler<TRequest, TResponse, THandler> : IForwardFacingFeatureSlice.Handler.Flag<TFeatureFlag, TRequest, TResponse, THandler>
            where THandler : class, IHandler<TRequest, TResponse>
        {
            public static void Register(IServiceCollection services)
            {
                RegisterInternal(services);
            }
        }
    }

    public abstract partial class WithEndpoint<TEndpoint> : IForwardFacingFeatureSlice.Endpoint.Default<TEndpoint>
        where TEndpoint : IEndpoint
    {
        public static void Register(HostExtender<WebApplication> hostExtender)
        {
            RegisterInternal(hostExtender);
        }

        public abstract partial class WithHandler<TRequest, TResponse, THandler> : IForwardFacingFeatureSlice.Handler.Default<TRequest, TResponse, THandler>
            where THandler : class, IHandler<TRequest, TResponse>
        {
            public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
            {
                WithEndpoint<TEndpoint>.Register(hostExtender);
                RegisterInternal(services);
            }
        }
    }

    public abstract partial class WithHandler<TRequest, TResponse, THandler> : IForwardFacingFeatureSlice.Handler.Default<TRequest, TResponse, THandler>
        where THandler : class, IHandler<TRequest, TResponse>
    {
        public static void Register(IServiceCollection services)
        {
            RegisterInternal(services);
        }
    }
}

public sealed class ExampleFeature : 
    FeatureSlice<ExampleFeature>
        .WithFlag<ExampleFeature>
        .WithEndpoint<ExampleFeature.Endpoint>
        .WithHandler<ExampleFeature.Request, ExampleFeature.Response, ExampleFeature.Handler>,
        IFeatureFlag
{
    public record Request();
    public record Response();

    public static string FeatureName => "ExampleFeature";

    public class Handler : IHandler<Request, Response>
    {
        public Task<OneOf<Response, Error>> Handle(Request response)
        {
            throw new NotImplementedException();
        }
    }

    public class Endpoint : EndpointHelper, IEndpoint
    {
        public static EndpointInfo Map => MapGet("test", (int age) => 
        {
            return Results.Ok();
        });
    }
}

public class Usage
{
    public static void Use(ExampleFeature.Dispatch dispatch)
    {
        dispatch.Invoke(new ExampleFeature.Request());
    }

    public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
    {
        ExampleFeature.Register(services, hostExtender);
    }
}

