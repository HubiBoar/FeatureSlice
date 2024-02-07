using Microsoft.AspNetCore.Builder;

namespace FeatureSlice.FluentGenerics.Interfaces2;

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
    public interface Default : IFeatureSlice
    {
        protected static void RegisterBase<TEndpoint>(HostExtender<WebApplication> hostExtender)
            where TEndpoint : IEndpoint
        {
            hostExtender.Map<TEndpoint>();
        }
    }

    public interface Flag : IFeatureSlice
    {
        protected static void RegisterBase<TFeatureFlag, TEndpoint>(HostExtender<WebApplication> hostExtender)
            where TFeatureFlag : IFeatureFlag
            where TEndpoint : IEndpoint
        {
            hostExtender.Map<TEndpoint>();
        }
    }
}