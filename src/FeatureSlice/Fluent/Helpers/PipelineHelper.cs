using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

public static class PipelineHelper
{
    public delegate TResponse Pipeline<TRequest, TResponse>(TRequest request, Next<TRequest, TResponse> next);
    public delegate TResponse Next<TRequest, TResponse>(TRequest request);

    public static TResponse RunPipelines<TRequest, TResponse>(
        TRequest request,
        IReadOnlyCollection<Pipeline<TRequest, TResponse>> pipelines,
        Next<TRequest, TResponse> lastMethod)
    {
        return RunNext(request, lastMethod, 0, pipelines.ToList());

        static TResponse RunNext(
            TRequest request,
            Next<TRequest, TResponse> lastMethod,
            int index,
            IReadOnlyList<Pipeline<TRequest, TResponse>> pipelines)
        {
            if (index < pipelines.Count)
            {
                return pipelines[index](request, r => RunNext(r, lastMethod, index++, pipelines));
            }
            else
            {
                return lastMethod.Invoke(request);
            }
        }
    }

    public static TResponse RunPipelines<TRequest, TResponse>(
        TRequest request,
        IServiceProvider provider,
        Next<TRequest, TResponse> lastMethod)
    {
        var pipelines = provider.GetServices<Pipeline<TRequest, TResponse>>().ToArray();

        return RunPipelines(request, pipelines, lastMethod);
    }
}