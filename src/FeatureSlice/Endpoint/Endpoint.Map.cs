using Definit.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

public static class FeatureSliceEndpointExtensions
{
    public static FeatureSliceBase<TRequest, TResult, TResponse>.ISetup Map<TSelf, TRequest, TResult, TResponse>
    (
        this FeatureSliceBase<TSelf, TRequest, TResult, TResponse>.HandleSetup options,
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
            services.AddSwaggerGen(options => options.SetCustomSchemaId());

            services.AddFeatureSlicesExtension<WebApplication>((host, provider) => 
            {
                var endpoint = new EndpointBuilder<TRequest, TResult, TResponse>
                (
                    method,
                    host,
                    provider =>
                        request =>
                            provider
                                .GetRequiredService<FeatureSliceBase<TSelf, TRequest, TResult, TResponse>.Dispatch>()(request)
                )
                {
                    Path = path
                };

                var handler = builder(endpoint);

                foreach(var extension in endpoint.Extensions)
                {
                    extension(handler);
                }

                return Task.CompletedTask;
            });
        });

        return options;
    }

    public static FeatureSliceBase<TRequest, TResult, TResponse>.ISetup MapGet<TSelf, TRequest, TResult, TResponse>
    (
        this FeatureSliceBase<TSelf, TRequest, TResult, TResponse>.HandleSetup options,
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

    public static FeatureSliceBase<TRequest, TResult, TResponse>.ISetup MapPost<TRequest, TResult, TResponse>
    (
        this FeatureSliceBase< TRequest, TResult, TResponse>.ISetup options,
        string path,
        Func<EndpointBuilder<TRequest, TResult, TResponse>, RouteHandlerBuilder> builder
    )
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        return options.Map(HttpMethod.Post, path, builder);
    }

    public static FeatureSliceBase<TRequest, TResult, TResponse>.ISetup MapPut<TSelf, TRequest, TResult, TResponse>
    (
        this FeatureSliceBase<TSelf, TRequest, TResult, TResponse>.HandleSetup options,
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

    public static FeatureSliceBase<TRequest, TResult, TResponse>.ISetup MapPatch<TSelf, TRequest, TResult, TResponse>
    (
        this FeatureSliceBase<TSelf, TRequest, TResult, TResponse>.HandleSetup options,
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

    public static FeatureSliceBase<TRequest, TResult, TResponse>.ISetup MapOptions<TSelf, TRequest, TResult, TResponse>
    (
        this FeatureSliceBase<TSelf, TRequest, TResult, TResponse>.HandleSetup options,
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

    public static FeatureSliceBase<TRequest, TResult, TResponse>.ISetup MapHead<TSelf, TRequest, TResult, TResponse>
    (
        this FeatureSliceBase<TSelf, TRequest, TResult, TResponse>.HandleSetup options,
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

    public static FeatureSliceBase<TRequest, TResult, TResponse>.ISetup MapTrace<TSelf, TRequest, TResult, TResponse>
    (
        this FeatureSliceBase<TSelf, TRequest, TResult, TResponse>.HandleSetup options,
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

    public static FeatureSliceBase<TRequest, TResult, TResponse>.ISetup MapDelete<TSelf, TRequest, TResult, TResponse>
    (
        this FeatureSliceBase<TSelf, TRequest, TResult, TResponse>.HandleSetup options,
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

    public static FeatureSliceBase<TRequest, TResult, TResponse>.ISetup MapConnect<TSelf, TRequest, TResult, TResponse>
    (
        this FeatureSliceBase<TSelf, TRequest, TResult, TResponse>.HandleSetup options,
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