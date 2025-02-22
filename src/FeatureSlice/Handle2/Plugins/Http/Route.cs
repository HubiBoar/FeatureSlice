namespace FeatureSlice.Handle2;

public interface IFeatureSliceRouteBuilder
{
    string Route { get; }
    string Method { get; }
    IFeatureSliceBuilder Builder { get; }
}

internal sealed record FeatureSliceRouteBuilder
(
    string Route,
    string Method,
    IFeatureSliceBuilder Builder
);

public static class FeatureSliceRouteBuilderExtension
{
    public static FeatureSliceRouteBuilder Route<TRequest>
    (
        this IFeatureSliceBuilder builder,
        string method, 
        string route
    )
    {
        return new FeatureSliceRouteBuilder(route, method, builder);
    }
}
