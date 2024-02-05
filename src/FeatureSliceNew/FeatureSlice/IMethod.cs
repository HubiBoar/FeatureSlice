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

    public interface IRegistrable<TSelf, TDispatcherType, TDispatcherInstance> : IMethod<TRequest, TResponse>
        where TSelf : class, IRegistrable<TSelf, TDispatcherType, TDispatcherInstance>
        where TDispatcherType : Delegate
        where TDispatcherInstance : Delegate
    {
        public static void Register(IServiceCollection services, Func<IServiceProvider, TDispatcherType> dispatch)
        {
            services.AddSingleton<TSelf>();
            services.AddSingleton<TDispatcherInstance>(provider => TSelf.Convert(dispatch(provider)));

            TSelf.OnRegistration(services, dispatch);
        }

        protected static abstract void OnRegistration(IServiceCollection services, Func<IServiceProvider, TDispatcherType> dispatch);

        public static abstract TDispatcherInstance Convert(TDispatcherType dispatch);
    }
}

public static class PipelineExtensions
{
    public static TResponse RunPipeline<TRequest, TResponse>(this IReadOnlyList<IMethod<TRequest, TResponse>.IPipeline> pipelines, TRequest request, IMethod<TRequest, TResponse>.IPipeline.Next featureMethod)
    {
        return IMethod<TRequest, TResponse>.IPipeline.RunPipeline(request, featureMethod, pipelines);
    }
}