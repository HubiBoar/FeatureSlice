
namespace FeatureSlice.Handle2;

public interface IFeatureSliceRouteBuilder<TRequest> : IFeatureSliceSetup
{
    string Route { get; }
    string Method { get; }
}

internal sealed record FeatureSliceRouteBuilder<TRequest>
(
    string Route,
    string Method
)
: IFeatureSliceRouteBuilder<TRequest>
{
    public void Configure(IServiceProvider provider)
    {
        throw new NotImplementedException();
    }
}

public static class FeatureSliceRouteBuilderExtension
{
    public static IFeatureSliceBuilder<IFeatureSliceRouteBuilder<TRequest>, IFeatureSliceRouteBuilder<TRequest>> Route<TRequest>
    (
        this IFeatureSliceBuilder builder,
        string method, 
        string route
    )
    {
        var ret = new FeatureSliceRouteBuilder<TRequest>(route, method);
        return new FeatureSliceBuilder<
            IFeatureSliceRouteBuilder<TRequest>,
            IFeatureSliceRouteBuilder<TRequest>>(
                builder,
                ret,
                ret);
    }
}
