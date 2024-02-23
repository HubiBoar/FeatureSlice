using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace FeatureSlice;

public interface IEndpoint
{
    public static abstract Setup Endpoint { get; }

    public sealed partial record Setup(Func<IEndpointRouteBuilder, IEndpointConventionBuilder> Extender) : IEndpointConventionBuilder
    {
        private readonly List<Action<EndpointBuilder>> _conventions = new ();

        public void Add(Action<EndpointBuilder> convention)
        {
            _conventions.Add(convention);
        }

        public IEndpointConventionBuilder Map(IEndpointRouteBuilder endpoint)
        {
            var builder = Extender(endpoint);
            foreach(var convention in _conventions)
            {
                builder.Add(convention);
            }

            return builder;
        }
    }

    public static Setup MapGet(string pattern, Delegate handler)
    {
        return new Setup(endpoint => endpoint.MapGet(pattern, handler));
    }

    public static Setup MapPost(string pattern, Delegate handler)
    {
        return new Setup(endpoint => endpoint.MapPost(pattern, handler));
    }

    public static Setup MapPut(string pattern, Delegate handler)
    {
        return new Setup(endpoint => endpoint.MapPut(pattern, handler));
    }

    public static Setup MapDelete(string pattern, Delegate handler)
    {
        return new Setup(endpoint => endpoint.MapDelete(pattern, handler));
    }
}

public static class EndpointExtensions
{
    public static IEndpointConventionBuilder Map<T>(this IEndpointRouteBuilder endpoint)
        where T : IEndpoint
    {
        return T.Endpoint.Map(endpoint);
    }

    public static WebAppExtender Map<T>(this WebAppExtender extender)
        where T : IEndpoint
    {
        extender.Map(builder => T.Endpoint.Map(builder));

        return extender;
    }
}