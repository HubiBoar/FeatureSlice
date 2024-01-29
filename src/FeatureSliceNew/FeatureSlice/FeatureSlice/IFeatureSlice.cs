using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

public partial interface IFeatureSlice<TRequest, TResponse> : IMethod<TRequest, Task<TResponse>>
{
    public static class Setup<TSelf>
        where TSelf : class, IFeatureSlice<TRequest, TResponse>
    {
        public static Task<TResponse> Dispatch(TRequest request, TSelf self, IReadOnlyList<IPipeline> pipelines)
        {
            return pipelines.RunPipeline(request, self.Handle);
        }

        public static Func<TRequest, Task<TResponse>> Factory(IServiceProvider provider)
        {
            return request => Dispatch(
                request,
                provider.GetRequiredService<TSelf>(),
                provider.GetServices<IPipeline>().ToList());
        }

        public static void Register<TDispatcher>(IServiceCollection services, Func<IServiceProvider, TDispatcher> factory)
            where TDispatcher : Delegate
        {
            services.AddSingleton<TSelf>();
        }
    }
}