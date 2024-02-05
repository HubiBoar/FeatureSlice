using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;

namespace FeatureSlice;

public sealed class HostExtender<THost>
    where THost : IHost
{
    private readonly List<Action<THost>> _extensions = new List<Action<THost>>();

    public void AddExtension(Action<THost> extension)
    {
        _extensions.Add(extension);
    }

    public void AddExtension(Action<IHost> extension)
    {
        _extensions.Add(host => extension(host));
    }

    public void Extend(THost host)
    {
        foreach(var extension in _extensions)
        {
            extension(host);
        }
    }
}

public static partial class Feature
{
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
}

public static class EndpointExtensions
{
    public static IEndpointConventionBuilder Map<T>(this IEndpointRouteBuilder endpoint)
        where T : Feature.IEndpoint
    {
        return T.Setup.Map(endpoint);
    }

    public static HostExtender<WebApplication> Map<T>(this HostExtender<WebApplication> extender)
        where T : Feature.IEndpoint
    {
        extender.AddExtension(host => T.Setup.Map(host));

        return extender;
    }
}