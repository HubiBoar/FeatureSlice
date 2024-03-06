using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace FeatureSlice;

public sealed class Endpoint : IEndpointConventionBuilder
{
    private readonly Func<IEndpointRouteBuilder, IEndpointConventionBuilder> _extender;
    private readonly List<Action<EndpointBuilder>> _conventions = new ();

    public Endpoint(Func<IEndpointRouteBuilder, IEndpointConventionBuilder> extender)
    {
        _extender = extender;
    }

    public void Add(Action<EndpointBuilder> convention)
    {
        _conventions.Add(convention);
    }

    internal IEndpointConventionBuilder Map(IEndpointRouteBuilder endpoint)
    {
        var builder = _extender(endpoint);
        foreach(var convention in _conventions)
        {
            builder.Add(convention);
        }

        return builder;
    }
}

public static class FeatureSliceEndpoint
{
    public static void AddEndpoint(
        IHostExtender<WebApplication> extender,
        Func<IEndpointRouteBuilder, IEndpointConventionBuilder> endpoint)
    {
        extender.Extend(host => endpoint(host));
    }

    public static void AddEndpoint(
        IHostExtender<WebApplication> extender,
        Endpoint endpoint)
    {
        extender.Extend(host => endpoint.Map(host));
    }
}

public static class Map
{
    public static Endpoint Get(string pattern, Delegate handler)
    {
        return new Endpoint(endpoint => endpoint.MapGet(pattern, handler));
    }

    public static Endpoint Post(string pattern, Delegate handler)
    {
        return new Endpoint(endpoint => endpoint.MapPost(pattern, handler));
    }

    public static Endpoint Put(string pattern, Delegate handler)
    {
        return new Endpoint(endpoint => endpoint.MapPut(pattern, handler));
    }

    public static Endpoint Delete(string pattern, Delegate handler)
    {
        return new Endpoint(endpoint => endpoint.MapDelete(pattern, handler));
    }
}

public interface IEndpointProvider
{
    public static abstract Endpoint Endpoint { get; }
}

public static class EndpointExtensions
{
    public static IEndpointConventionBuilder Map<T>(this IEndpointRouteBuilder endpoint)
        where T : IEndpointProvider
    {
        return T.Endpoint.Map(endpoint);
    }

    public static WebAppExtender Map<T>(this WebAppExtender extender)
        where T : IEndpointProvider
    {
        extender.Map<T>();

        return extender;
    }

    public static WebAppExtender Map(this WebAppExtender extender, Endpoint endpoint)
    {
        extender.Map(host => endpoint.Map(host));

        return extender;
    }
}