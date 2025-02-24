namespace FeatureSlice.Handle2;

public sealed record Html(string Value)
{
    public static Html Empty { get; } = new Html(string.Empty);
}

public interface IFeatureSliceHtml<TRequest> : IFeatureSliceSetup, IFeatureSliceRouteBuilder<TRequest>
{
    Html GetHtml(TRequest request);
}

internal sealed record FeatureSliceHtml<TRequest>(IFeatureSliceRouteBuilder<TRequest> Builder, Func<TRequest, Html> Get) : IFeatureSliceHtml<TRequest>
{
    public string Route => Builder.Route;

    public string Method => Builder.Method;

    public void Configure(IServiceProvider provider) {}

    public Html GetHtml(TRequest request) => Get(request);
}

public static class Extension
{
    public static IFeatureSliceBuilder<IFeatureSliceHtml<TRequest>, IFeatureSliceRouteBuilder<TRequest>> Html<TRequest>
    (
        this IFeatureSliceBuilder.IWithMetadata<IFeatureSliceRouteBuilder<TRequest>> builder,
        Func<TRequest, Html> get
    )
    {
        return new FeatureSliceBuilder<IFeatureSliceHtml<TRequest>, IFeatureSliceRouteBuilder<TRequest>>(
            new FeatureSliceHtml<TRequest>(builder.Metadata, get),
            builder.Metadata);
    }

    public static Html Htmx<T, TRequest>(this Html html)
        where T : IFeatureSliceBase.IRequest<TRequest>
    {
        return html;
    }
}
