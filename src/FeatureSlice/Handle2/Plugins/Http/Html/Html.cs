using Microsoft.AspNetCore.Builder;

namespace FeatureSlice.Handle2;

public sealed record Html(string Value)
{
    public static Html Empty { get; } = new Html(string.Empty);
}

public interface IFeatureSliceHtml<TRequest> : IFeatureSliceSetup, IRouteBuilder
{
    Html GetHtml(TRequest request);
}

internal sealed record FeatureSliceHtml<TRequest>(IRouteBuilder Builder, Func<TRequest, Html> Get) : IFeatureSliceHtml<TRequest>
{
    public string Route => Builder.Route;

    public HttpMethod Method => Builder.Method;

    public void Configure(IServiceProvider provider) {}

    public void Extend(Action<RouteHandlerBuilder> builder) => Builder.Extend(builder);

    public Html GetHtml(TRequest request) => Get(request);
}

public static class Extension
{
    public static FeatureSlice<TRequest, TResponse>.Config<IFeatureSliceHtml<TRequest>, IRouteBuilder> Html<TRequest, TResponse>
    (
        this FeatureSlice<TRequest, TResponse>.IConfig.IMetadata<IRouteBuilder> builder,
        Func<TRequest, Html> get
    )
    {
        return new (new FeatureSliceHtml<TRequest>(builder.Metadata, get), builder.Metadata, builder.Builder);
    }

    public static Html Htmx<T, TRequest>(this Html html)
        where T : IFeatureSliceBase.IRequest<TRequest>
    {
        return html;
    }
}
