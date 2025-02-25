using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace FeatureSlice.Handle2;

public interface IRouteBuilder : IFeatureSliceSetup
{
    public HttpMethod Method { get; }
    string Route { get; }

    public void Extend(Action<RouteHandlerBuilder> builder);
}

internal sealed record RouteBuilder
(
    HttpMethod Method, 
    string Route
)
: IRouteBuilder
{
    public void Configure(IServiceProvider provider)
    {
    }

    public void Extend(Action<RouteHandlerBuilder> builder)
    {
    }
}

public static class FeatureSliceRouteBuilderExtension
{
    public static FeatureSlice<TRequest, TResponse>.Config<IRouteBuilder, IRouteBuilder> Route<TRequest, TResponse>
    (
        this FeatureSlice<TRequest, TResponse>.IConfig config,
        HttpMethod method, 
        string route
    )
    {
        var routeBuilder = new RouteBuilder(method, route);
        return new (routeBuilder, routeBuilder, config.Builder);
    }
}
