using Definit.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

public static class FeatureSliceEndpointExtensions
{
    public static void MapFeatureSlices(this IEndpointRouteBuilder endpointRoute)
    {
        var services = endpointRoute.ServiceProvider.GetServices<EndpointMapper>();

        foreach(var service in services)
        {
            service.Map(endpointRoute);
        }
    }

    public static FeatureSliceBase<TSelf, TRequest, TResult, TResponse>.Options Map<TSelf, TRequest, TResult, TResponse>
    (
        this FeatureSliceBase<TSelf, TRequest, TResult, TResponse>.Options options,
        HttpMethod method,
        string path,
        Func<EndpointBuilder<TRequest, TResult, TResponse>, RouteHandlerBuilder> builder
    )
        where TSelf : FeatureSliceBase<TSelf, TRequest, TResult, TResponse>, new()
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        options.Extend(services => 
        {
            services.AddSingleton(new EndpointMapper(route => 
            {
                var endpoint = new EndpointBuilder<TRequest, TResult, TResponse>
                (
                    method,
                    route,
                    provider => request => provider.GetRequiredService<FeatureSliceBase<TSelf, TRequest, TResult, TResponse>.Dispatch>()(request)
                )
                {
                    Path = path
                };

                var handler = builder(endpoint);

                foreach(var extension in endpoint.Extensions)
                {
                    extension(handler);
                }
            }));
        });

        return options;
    }

    public static FeatureSliceBase<TSelf, TRequest, TResult, TResponse>.Options MapGet<TSelf, TRequest, TResult, TResponse>
    (
        this FeatureSliceBase<TSelf, TRequest, TResult, TResponse>.Options options,
        string path,
        Func<EndpointBuilder<TRequest, TResult, TResponse>, RouteHandlerBuilder> builder
    )
        where TSelf : FeatureSliceBase<TSelf, TRequest, TResult, TResponse>, new()
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        return options.Map(HttpMethod.Get, path, builder);
    }

    public static FeatureSliceBase<TSelf, TRequest, TResult, TResponse>.Options MapPost<TSelf, TRequest, TResult, TResponse>
    (
        this FeatureSliceBase<TSelf, TRequest, TResult, TResponse>.Options options,
        string path,
        Func<EndpointBuilder<TRequest, TResult, TResponse>, RouteHandlerBuilder> builder
    )
        where TSelf : FeatureSliceBase<TSelf, TRequest, TResult, TResponse>, new()
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        return options.Map(HttpMethod.Post, path, builder);
    }

    public static FeatureSliceBase<TSelf, TRequest, TResult, TResponse>.Options MapPut<TSelf, TRequest, TResult, TResponse>
    (
        this FeatureSliceBase<TSelf, TRequest, TResult, TResponse>.Options options,
        string path,
        Func<EndpointBuilder<TRequest, TResult, TResponse>, RouteHandlerBuilder> builder
    )
        where TSelf : FeatureSliceBase<TSelf, TRequest, TResult, TResponse>, new()
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        return options.Map(HttpMethod.Put, path, builder);
    }

    public static FeatureSliceBase<TSelf, TRequest, TResult, TResponse>.Options MapPatch<TSelf, TRequest, TResult, TResponse>
    (
        this FeatureSliceBase<TSelf, TRequest, TResult, TResponse>.Options options,
        string path,
        Func<EndpointBuilder<TRequest, TResult, TResponse>, RouteHandlerBuilder> builder
    )
        where TSelf : FeatureSliceBase<TSelf, TRequest, TResult, TResponse>, new()
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        return options.Map(HttpMethod.Patch, path, builder);
    }

    public static FeatureSliceBase<TSelf, TRequest, TResult, TResponse>.Options MapOptions<TSelf, TRequest, TResult, TResponse>
    (
        this FeatureSliceBase<TSelf, TRequest, TResult, TResponse>.Options options,
        string path,
        Func<EndpointBuilder<TRequest, TResult, TResponse>, RouteHandlerBuilder> builder
    )
        where TSelf : FeatureSliceBase<TSelf, TRequest, TResult, TResponse>, new()
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        return options.Map(HttpMethod.Options, path, builder);
    }

    public static FeatureSliceBase<TSelf, TRequest, TResult, TResponse>.Options MapHead<TSelf, TRequest, TResult, TResponse>
    (
        this FeatureSliceBase<TSelf, TRequest, TResult, TResponse>.Options options,
        string path,
        Func<EndpointBuilder<TRequest, TResult, TResponse>, RouteHandlerBuilder> builder
    )
        where TSelf : FeatureSliceBase<TSelf, TRequest, TResult, TResponse>, new()
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        return options.Map(HttpMethod.Head, path, builder);
    }

    public static FeatureSliceBase<TSelf, TRequest, TResult, TResponse>.Options MapTrace<TSelf, TRequest, TResult, TResponse>
    (
        this FeatureSliceBase<TSelf, TRequest, TResult, TResponse>.Options options,
        string path,
        Func<EndpointBuilder<TRequest, TResult, TResponse>, RouteHandlerBuilder> builder
    )
        where TSelf : FeatureSliceBase<TSelf, TRequest, TResult, TResponse>, new()
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        return options.Map(HttpMethod.Trace, path, builder);
    }

    public static FeatureSliceBase<TSelf, TRequest, TResult, TResponse>.Options MapDelete<TSelf, TRequest, TResult, TResponse>
    (
        this FeatureSliceBase<TSelf, TRequest, TResult, TResponse>.Options options,
        string path,
        Func<EndpointBuilder<TRequest, TResult, TResponse>, RouteHandlerBuilder> builder
    )
        where TSelf : FeatureSliceBase<TSelf, TRequest, TResult, TResponse>, new()
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        return options.Map(HttpMethod.Delete, path, builder);
    }

    public static FeatureSliceBase<TSelf, TRequest, TResult, TResponse>.Options MapConnect<TSelf, TRequest, TResult, TResponse>
    (
        this FeatureSliceBase<TSelf, TRequest, TResult, TResponse>.Options options,
        string path,
        Func<EndpointBuilder<TRequest, TResult, TResponse>, RouteHandlerBuilder> builder
    )
        where TSelf : FeatureSliceBase<TSelf, TRequest, TResult, TResponse>, new()
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        return options.Map(HttpMethod.Connect, path, builder);
    }
}