using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Definit.Endpoint;

namespace FeatureSlice;

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
        extender.Map(endpoint);
    }
}