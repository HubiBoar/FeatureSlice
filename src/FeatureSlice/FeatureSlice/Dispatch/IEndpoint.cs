using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;

namespace FeatureSlice.Dispatch;

public sealed partial record Endpoint(Func<IEndpointRouteBuilder, IEndpointConventionBuilder> Setup) : IEndpointConventionBuilder
{
    private readonly List<Action<EndpointBuilder>> _conventions = new List<Action<EndpointBuilder>>();

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

public sealed partial record Endpoint
{
    public static Endpoint MapGet(string pattern, Delegate handler)
    {
        return new Endpoint(endpoint => endpoint.MapGet(pattern, handler));
    }

    public static Endpoint MapPost(string pattern, Delegate handler)
    {
        return new Endpoint(endpoint => endpoint.MapPost(pattern, handler));
    }

    public static Endpoint MapPut(string pattern, Delegate handler)
    {
        return new Endpoint(endpoint => endpoint.MapPut(pattern, handler));
    }

    public static Endpoint MapDelete(string pattern, Delegate handler)
    {
        return new Endpoint(endpoint => endpoint.MapDelete(pattern, handler));
    }
}

public interface IEndpoint
{
    public static abstract Endpoint Setup { get; }
}

public static class EndpointExtensions
{
    public static IEndpointConventionBuilder Map<T>(this IEndpointRouteBuilder endpoint)
        where T : IEndpoint
    {
        return T.Setup.Map(endpoint);
    }
}

public sealed class Example : IEndpoint
{
    public static Endpoint Setup => Endpoint.MapGet("/test", TestMethod)
        .WithName("name");

    private static IResult TestMethod(string param)
    {
        return Results.Ok();
    }
}