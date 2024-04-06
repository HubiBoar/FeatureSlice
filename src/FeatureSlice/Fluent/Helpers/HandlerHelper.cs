using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using Definit.Results;

namespace FeatureSlice;

public delegate T ServiceFactory<T>(IServiceProvider provider);

public delegate Task<TResponse> HandlerPipeline<TRequest, TResponse>(TRequest request, Handler<TRequest, TResponse> next) where TRequest : notnull where TResponse : notnull;
public delegate Task<TResponse> Handler<TRequest, TResponse>(TRequest request) where TRequest : notnull where TResponse : notnull;
public delegate Task<TResponse> Handler<TRequest, TResponse, TDependencies>(TRequest request, TDependencies dependencies) where TRequest : notnull where TResponse : notnull;

public static class HandlerHelper
{
    public static Handler<TRequest, Result<TResponse>> RunWithPipelines<TRequest, TResponse>(
        IServiceProvider provider,
        ServiceFactory<Handler<TRequest, Result<TResponse>>> factory)
        where TRequest : notnull
        where TResponse : notnull
    {
        var handler = factory(provider);

        return request => PipelineHelper.RunPipelines(request, provider, handler.Invoke);
    }

    public static Handler<TRequest, Result<TResponse, Disabled>> RunWithPipelinesAndFlag<TRequest, TResponse>(
        string featureName,
        IServiceProvider provider,
        ServiceFactory<Handler<TRequest, Result<TResponse>>> factory)
        where TRequest : notnull
        where TResponse : notnull
    {
        var handler = factory(provider);

        return request => PipelineHelper.RunPipelines(request, provider, ConvertToHandlerWithFlag(featureName, provider, handler).Invoke);
    }

    public static Handler<TRequest, Result<TResponse, Disabled>> ConvertToHandlerWithFlag<TRequest, TResponse>(
        string featureName,
        IServiceProvider provider,
        Handler<TRequest, Result<TResponse>> handler)
        where TRequest : notnull
        where TResponse : notnull
    {
        return async request => {

            var manager = provider.GetRequiredService<IFeatureManager>();

            if(await manager.IsEnabledAsync(featureName))
            {
                return new Disabled($"FeatureFlag [{featureName}] is Disabled");
            }

            return await PipelineHelper.RunPipelines(request, provider, handler.Invoke);
        };
    }

    public static IPublisher.Listen<TRequest> ConvertToListener<TRequest, TResponse>(
        Handler<TRequest, Result<TResponse>> handler)
        where TRequest : notnull
        where TResponse : notnull
    {
        return async request => await handler(request);
    }


    public static Handler<TRequest, Result> RunWithPipelines<TRequest>(
        IServiceProvider provider,
        ServiceFactory<Handler<TRequest, Result>> factory)
        where TRequest : notnull
    {
        var handler = factory(provider);

        return request => PipelineHelper.RunPipelines(request, provider, handler.Invoke);
    }

    public static Handler<TRequest, Result.Or<Disabled>> RunWithPipelinesAndFlag<TRequest>(
        string featureName,
        IServiceProvider provider,
        ServiceFactory<Handler<TRequest, Result>> factory)
        where TRequest : notnull
    {
        var handler = factory(provider);

        return request => PipelineHelper.RunPipelines(request, provider, ConvertToHandlerWithFlag(featureName, provider, handler).Invoke);
    }

    public static Handler<TRequest, Result.Or<Disabled>> ConvertToHandlerWithFlag<TRequest>(
        string featureName,
        IServiceProvider provider,
        Handler<TRequest, Result> handler)
        where TRequest : notnull
    {
        return async request => {

            var manager = provider.GetRequiredService<IFeatureManager>();

            if(await manager.IsEnabledAsync(featureName))
            {
                return new Disabled($"FeatureFlag [{featureName}] is Disabled");
            }

            return await PipelineHelper.RunPipelines(request, provider, handler.Invoke);
        };
    }

    public static IPublisher.Listen<TRequest> ConvertToListener<TRequest>(
        Handler<TRequest, Result> handler)
        where TRequest : notnull
    {
        return async request => await handler(request);
    }

    public static IPublisher.Listen<TRequest> ConvertToListener<TRequest>(
        Messaging.Dispatch<TRequest> dispatch)
        where TRequest : notnull
    {
        return async request => await dispatch(request);
    }

    public static Messaging.Consume<TRequest> ConvertToConsume<TRequest>(
        Handler<TRequest, Result> handler)
        where TRequest : notnull
    {
        return async request => await handler(request);
    }
}