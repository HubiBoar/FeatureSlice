using Microsoft.AspNetCore.Builder;

namespace FeatureSlice.FluentGenerics;

public sealed record EndpointInfo(HttpMethod Method, string Pattern, Delegate Handler);

public interface IEndpoint
{
    public static abstract EndpointInfo Map { get; }

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
        var endpointInfo = T.Map;
        extender.AddExtension(host => host.MapMethods(endpointInfo.Pattern, [ endpointInfo.Method.ToString() ], endpointInfo.Handler));

        return extender;
    }
}

public static class EndpointFeatureSlice
{
    public abstract partial class WithEndpointBase<TEndpoint> : Default<TEndpoint>
        where TEndpoint : IEndpoint
    {
        protected static new void RegisterInternal(HostExtender<WebApplication> hostExtender)
        {
            Default<TEndpoint>.RegisterInternal(hostExtender);
        }
    }

    public abstract partial class Default<TEndpoint> : IFeatureSlice
        where TEndpoint : IEndpoint
    {
        protected static void RegisterInternal(HostExtender<WebApplication> hostExtender)
        {
            hostExtender.Map<TEndpoint>();
        }
    }

    public abstract partial class Flag<TFeatureFlag, TEndpoint> : IFeatureSlice
        where TFeatureFlag : IFeatureFlag
        where TEndpoint : IEndpoint
    {
        protected static void RegisterInternal(HostExtender<WebApplication> hostExtender)
        {
            hostExtender.Map<TEndpoint>();
        }
    }
}