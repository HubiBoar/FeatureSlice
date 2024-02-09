namespace FeatureSlice;

public interface IPipeline<TRequest, TResponse>
{
    public delegate TResponse Next(TRequest request);

    public TResponse Handle(TRequest request, Next next);

    public static TResponse RunPipeline(
        TRequest request,
        Next featureMethod,
        IReadOnlyList<IPipeline<TRequest, TResponse>> pipelines)
    {
        return RunPipeline(request, featureMethod, 0, pipelines);
    }

    private static TResponse RunPipeline(
        TRequest request,
        Next lastMethod,
        int index,
        IReadOnlyList<IPipeline<TRequest, TResponse>> pipelines)
    {
        if (index < pipelines.Count)
        {
            return pipelines[index].Handle(request, r => RunPipeline(r, lastMethod, index++, pipelines));
        }
        else
        {
            return lastMethod.Invoke(request);
        }
    }
}

public static class PipelineExtensions
{
    public static TResponse RunPipeline<TRequest, TResponse>(this IReadOnlyList<IPipeline<TRequest, TResponse>> pipelines, TRequest request, IPipeline<TRequest, TResponse>.Next featureMethod)
    {
        return IPipeline<TRequest, TResponse>.RunPipeline(request, featureMethod, pipelines);
    }
}