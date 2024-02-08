using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

public interface IMethod<TRequest, TResponse>
{
    public TResponse Handle(TRequest request);

    public interface IPipeline
    {
        public delegate TResponse Next(TRequest request);

        public TResponse Handle(TRequest request, Next next);

        public static TResponse RunPipeline(
            TRequest request,
            Next featureMethod,
            IReadOnlyList<IPipeline> pipelines)
        {
            return RunPipeline(request, featureMethod, 0, pipelines);
        }

        private static TResponse RunPipeline(
            TRequest request,
            Next lastMethod,
            int index,
            IReadOnlyList<IPipeline> pipelines)
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
}

public static class PipelineExtensions
{
    public static TResponse RunPipeline<TRequest, TResponse>(this IReadOnlyList<IMethod<TRequest, TResponse>.IPipeline> pipelines, TRequest request, IMethod<TRequest, TResponse>.IPipeline.Next featureMethod)
    {
        return IMethod<TRequest, TResponse>.IPipeline.RunPipeline(request, featureMethod, pipelines);
    }
}