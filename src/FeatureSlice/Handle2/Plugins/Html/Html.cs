namespace FeatureSlice.Handle2;

public sealed record Html(string Value)
{
    public static Html Empty { get; } = new Html(string.Empty);
}

public interface IFeatureSliceHtml<TRequest> : IFeatureSliceSetup
{
    string Route { get; }
    string Method { get; }

    Html GetHtml(TRequest request);
}

public sealed record FeatureSliceHtml<TRequest>(string Route, string Method, Func<TRequest, Html> Get) : IFeatureSliceHtml<TRequest>
{
    public void Configure(IServiceProvider provider) {}

    public Html GetHtml(TRequest request) => Get(request);
}

public static class Extension
{
    public static FeatureSliceBuilder<IFeatureSliceHtml<TRequest>> Html<TRequest>
    (
        this IFeatureSliceBuilder builder,
        string method, 
        string route,
        Func<TRequest, Html> get
    )
    {
        return new FeatureSliceBuilder<IFeatureSliceHtml<TRequest>>(builder, new FeatureSliceHtml<TRequest>(route, method, get));
    }

    public static Html Htmx<T, TRequest>(this Html html)
        where T : IFeatureSliceBase.IRequest<TRequest>
    {
        return html;
    }
}
