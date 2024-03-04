using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using OneOf;
using OneOf.Types;

namespace FeatureSlice;

public struct Disabled();

public delegate T ServiceFactory<T>(IServiceProvider provider);

public delegate Task<OneOf<TResponse, Error>> HandlerPipeline<TRequest, TResponse>(TRequest request, Handler<TRequest, TResponse> next);
public delegate Task<OneOf<TResponse, Error>> Handler<TRequest, TResponse>(TRequest request);
public delegate Task<OneOf<TResponse, Error>> Handler<TRequest, TResponse, TDependencies>(TRequest request, TDependencies dependencies);

public delegate Task<OneOf<TResponse, Disabled, Error>> HandlerWithFlagPipeline<TRequest, TResponse>(TRequest request, HandlerWithFlag<TRequest, TResponse> next);
public delegate Task<OneOf<TResponse, Disabled, Error>> HandlerWithFlag<TRequest, TResponse>(TRequest request);
public delegate Task<OneOf<TResponse, Disabled, Error>> HandlerWithFlag<TRequest, TResponse, TDependencies>(TRequest request, TDependencies dependencies);

public static class HandlerHelper
{
    public static Handler<TRequest, TResponse> RunWithPipelines<TRequest, TResponse>(
        IServiceProvider provider,
        ServiceFactory<Handler<TRequest, TResponse>> factory)
    {
        var handler = factory(provider);

        return request => PipelineHelper.RunPipelines(request, provider, handler.Invoke);
    }

    public static HandlerWithFlag<TRequest, TResponse> RunWithPipelinesAndFlag<TRequest, TResponse>(
        string featureName,
        IServiceProvider provider,
        ServiceFactory<Handler<TRequest, TResponse>> factory)
    {
        var handler = factory(provider);

        return request => PipelineHelper.RunPipelines(request, provider, ConvertToHandlerWithFlag(featureName, provider, handler).Invoke);
    }

    public static HandlerWithFlag<TRequest, TResponse> ConvertToHandlerWithFlag<TRequest, TResponse>(string featureName, IServiceProvider provider, Handler<TRequest, TResponse> handler)
    {
        return async request => {

            var manager = provider.GetRequiredService<IFeatureManager>();

            if(await manager.IsEnabledAsync(featureName))
            {
                return new Disabled();
            }

            var result = await PipelineHelper.RunPipelines(request, provider, handler.Invoke);

            return result.Match<OneOf<TResponse, Disabled, Error>>(response => response, error => error);
        };
    }

    public static IPublisher.Listen<TRequest> ConvertToListener<TRequest, TResponse>(Handler<TRequest, TResponse> handler)
    {
        return async request => {

            var result = await handler(request);

            return result.Match<OneOf<Success, Error>>(success => new Success(), error => error);
        };
    }

    public static IPublisher.Listen<TRequest> ConvertToListener<TRequest>(Messaging.Dispatch<TRequest> dispatch)
    {
        return async request => {

            var result = await dispatch(request);

            return result.Match<OneOf<Success, Error>>(success => success, disabled => new Success(), error => error);
        };
    }

    public static Messaging.Consume<TRequest> ConvertToConsume<TRequest>(Handler<TRequest, Success> handler)
    {
        return async request => {
            var result = await handler(request);

            return result.Match<OneOf<Success, Disabled, Error>>(success => success, error => error);
        };
    }
}