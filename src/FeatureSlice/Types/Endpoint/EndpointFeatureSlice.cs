using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;

namespace FeatureSlice;

public static class EndpointFeatureSlice
{
    public interface Default<TEndpoint> : IFeatureSlice
        where TEndpoint : IEndpoint
    {
        protected static void RegisterBase(HostExtender<WebApplication> hostExtender)
        {
            hostExtender.Map<TEndpoint>();
        }
    }

    public interface Flag<TFeatureFlag, TEndpoint> : IFeatureSlice
        where TFeatureFlag : IFeatureFlag
        where TEndpoint : IEndpoint
    {
        protected static void RegisterBase(HostExtender<WebApplication> hostExtender)
        {
            hostExtender.Map<TEndpoint>();
        }
    }
}