using Microsoft.AspNetCore.Builder;

namespace FeatureSlice.FluentGenerics.Interfaces;

public sealed record EndpointInfo(HttpMethod Method, string Pattern, Delegate Handler);

public interface IEndpoint
{
    public static abstract EndpointInfo Map { get; }

    public static EndpointInfo Get(string pattern, Delegate handler)
    {
        return new EndpointInfo(HttpMethod.Get, pattern, handler);
    }
}

public abstract class EndpointHelper
{
    public static EndpointInfo Get(string pattern, Delegate handler) => IEndpoint.Get(pattern, handler);
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

public static class EndpointFeatureSlice
{
    public interface Default<TEndpoint> : IFeatureSlice
        where TEndpoint : IEndpoint
    {
        protected static void RegisterInternal(HostExtender<WebApplication> hostExtender)
        {
            hostExtender.Map<TEndpoint>();
        }
    }

    public interface AsDefault : IFeatureSlice, IEndpoint
    {
        protected static void RegisterInternal<TSelf>(HostExtender<WebApplication> hostExtender)
            where TSelf : AsDefault
        {
            hostExtender.Map<TSelf>();
        }
    }

    public interface Flag<TEndpoint> : IFeatureSlice, IFeatureFlag
        where TEndpoint : IEndpoint
    {
        protected static void RegisterInternal<TFlag>(HostExtender<WebApplication> hostExtender)
            where TFlag : IFeatureFlag
        {
            hostExtender.Map<TEndpoint>();
        }
    }

    public interface AsFlag : IFeatureSlice, IFeatureFlag, IEndpoint
    {
        protected static void RegisterInternal<TSelf>(HostExtender<WebApplication> hostExtender)
            where TSelf : AsFlag
        {
            hostExtender.Map<TSelf>();
        }
    }
}