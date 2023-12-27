using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FeatureSlice;

public interface IFeatureSlicePipelineBase<TBase>
{
    public static abstract void RegisterPipeline<T>(IServiceCollection collection)
        where T : class, TBase;
}

public interface IFeatureSlicePipeline<TRequest, TResponse> : IFeatureSlicePipelineBase<
    IFeatureSlicePipeline<TRequest, TResponse>>
{
    public Task<TResponse> Handle(TRequest request, Func<TRequest, Task<TResponse>> next);

    static void IFeatureSlicePipelineBase<IFeatureSlicePipeline<TRequest, TResponse>>.RegisterPipeline<T>(
        IServiceCollection collection)
    {
        collection.TryAddSingleton<IFeatureSlicePipeline<TRequest, TResponse>, T>();
    }
}