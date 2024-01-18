using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice.Dispatch;

public interface IEndpoint
{
    public static abstract EndpointSetup Setup { get; }
}

public sealed partial record EndpointSetup(Func<IEndpointRouteBuilder, IEndpointConventionBuilder> Setup) : IEndpointConventionBuilder
{
    private readonly List<Action<EndpointBuilder>> _conventions = new ();

    public void Add(Action<EndpointBuilder> convention)
    {
        _conventions.Add(convention);
    }

    public IEndpointConventionBuilder Map(IEndpointRouteBuilder endpoint)
    {
        var builder = Setup(endpoint);
        foreach(var convention in _conventions)
        {
            builder.Add(convention);
        }

        return builder;
    }
}

public sealed partial record EndpointSetup
{
    public static EndpointSetup MapGet(string pattern, Delegate handler)
    {
        return new EndpointSetup(endpoint => endpoint.MapGet(pattern, handler));
    }

    public static EndpointSetup MapPost(string pattern, Delegate handler)
    {
        return new EndpointSetup(endpoint => endpoint.MapPost(pattern, handler));
    }

    public static EndpointSetup MapPut(string pattern, Delegate handler)
    {
        return new EndpointSetup(endpoint => endpoint.MapPut(pattern, handler));
    }

    public static EndpointSetup MapDelete(string pattern, Delegate handler)
    {
        return new EndpointSetup(endpoint => endpoint.MapDelete(pattern, handler));
    }
}


public static class EndpointExtensions
{
    public static IEndpointConventionBuilder Map<T>(this IEndpointRouteBuilder endpoint)
        where T : IEndpoint
    {
        return T.Setup.Map(endpoint);
    }

    public static void Map<T>(this IServiceCollection services)
        where T : IEndpoint
    {
        services.AddHostExtension<WebApplication>(endpoint => T.Setup.Map(endpoint));
    }
}