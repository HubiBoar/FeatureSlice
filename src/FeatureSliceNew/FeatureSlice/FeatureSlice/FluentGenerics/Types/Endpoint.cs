using Microsoft.AspNetCore.Builder;

namespace FeatureSlice.FluentGenerics;

public sealed record EndpointInfo(HttpMethod Method, string Pattern, Delegate Handler);

public interface IEndpoint
{
    public static abstract EndpointInfo Info { get; }

    public static EndpointInfo MapGet(string pattern, Delegate handler)
    {
        return new EndpointInfo(HttpMethod.Get, pattern, handler);
    }
}

public abstract class EndpointHelper
{
    public static EndpointInfo MapGet(string pattern, Delegate handler) => IEndpoint.MapGet(pattern, handler);
}

public static class EndpointExtensions
{
    public static HostExtender<WebApplication> Map<T>(this HostExtender<WebApplication> extender)
        where T : IEndpoint
    {
        var endpointInfo = T.Info;
        extender.AddExtension(host => host.MapMethods(endpointInfo.Pattern, [ endpointInfo.Method.ToString() ], endpointInfo.Handler));

        return extender;
    }
}

public static class EndpointFeatureSlice
{
    public interface Default<TEndpoint> : IFeatureSlice
        where TEndpoint : IEndpoint
    {
        public static void Register(HostExtender<WebApplication> hostExtender)
        {
            hostExtender.Map<TEndpoint>();
        }
    }

    public interface Flag<TFeatureFlag, TEndpoint> : IFeatureSlice
        where TFeatureFlag : IFeatureFlag
        where TEndpoint : IEndpoint
    {
        public static void Register(HostExtender<WebApplication> hostExtender)
        {
            hostExtender.Map<TEndpoint>();
        }
    }
}