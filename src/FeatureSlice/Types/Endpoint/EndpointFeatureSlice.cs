using Explicit.Configuration;
using Microsoft.AspNetCore.Builder;

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

    public interface Flag<TFeatureName, TEndpoint> : IFeatureSlice
        where TFeatureName : IFeatureName
        where TEndpoint : IEndpoint
    {
        protected static void RegisterBase(HostExtender<WebApplication> hostExtender)
        {
            hostExtender.Map<TEndpoint>();
        }
    }
}