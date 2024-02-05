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

public partial interface IFeatureSliceBase<TSelf, TRequest, TResponse, THandler>
    where TSelf : IFeatureSliceBase<TSelf, TRequest, TResponse, THandler>
    where THandler : IHandler<TRequest, TResponse>
{
}

public abstract partial class FeatureSlice<TSelf, TRequest, TResponse, THandler>
    where TSelf : IFeatureSliceBase<TSelf, TRequest, TResponse, THandler>
    where THandler : IHandler<TRequest, TResponse>
{
    public delegate Task<OneOf<TRequest, Error>> Dispatch(TResponse request);

    public abstract partial class WithToggle : IFeatureSliceBase<TSelf, TRequest, TResponse, THandler>
    {
        public delegate Task<OneOf<TRequest, Disabled, Error>> Dispatch(TResponse request);

        public abstract string Name { get; }

        public abstract partial class WithEndpoint<TEndpoint> : IFeatureSliceBase<TSelf, TRequest, TResponse, THandler>
            where TEndpoint : Feature.IEndpoint
        {
            public delegate Task<OneOf<TRequest, Disabled, Error>> Dispatch(TResponse request);

            public abstract string Name { get; }

            public static void Register(IServiceCollection services, IConfiguration configuration, HostExtender<WebApplication> hostExtender)
            {
                WithToggle.Register(services, configuration);
                hostExtender.Map<TEndpoint>();
            }
        }

        public static void Register(IServiceCollection services, IConfiguration configuration)
        {
        }
    }

    public abstract partial class WithEndpoint<TEndpoint> : IFeatureSliceBase<TSelf, TRequest, TResponse, THandler>
        where TEndpoint : FeatureSlice.Feature.IEndpoint
    {
        public static void Register(IServiceCollection services, IConfiguration configuration)
        {
        }
    }

    public static void Register(IServiceCollection services, IConfiguration configuration)
    {
    }
}

public sealed class ExampleFeature : FeatureSlice<
    ExampleFeature,
    ExampleFeature.Request,
    ExampleFeature.Response,
    ExampleFeature.Handler>
    .WithToggle
    .WithEndpoint<ExampleFeature.Endpoint>
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

    public class Endpoint : FeatureSlice.Feature.IEndpoint
    {
        public static FeatureSlice.Feature.EndpointSetup Setup =>
            FeatureSlice.Feature.EndpointSetup.MapPut("test", () => {});
    }
}

public class Usage
{
    public static void Use(ExampleFeature.Dispatch dispatch)
    {
        dispatch.Invoke(new ExampleFeature.Response());
    }

    public static void Register(IServiceCollection services, IConfiguration configuration)
    {
        ExampleFeature.Register(services, configuration);
    }
}

