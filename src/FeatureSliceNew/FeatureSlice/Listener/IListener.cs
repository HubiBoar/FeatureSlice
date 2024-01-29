using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

namespace FeatureSlice;

public partial interface IListener<TRequest> : IMethod<TRequest, Task>
{
    public delegate Task Dispatch(TRequest request);

    protected virtual Task HandleListener(TRequest request, IFeatureManager featureManager)
    {
        return Handle(request);
    }

    public static class Setup<TSelf>
        where TSelf : class, IListener<TRequest>
    {
        public static Task Dispatch(TRequest request, IReadOnlyCollection<IListener<TRequest>> listeners, IFeatureManager manager, IReadOnlyList<IPipeline> pipelines)
        {
            return pipelines.RunPipeline(request, async r => {
                foreach(var listener in listeners)
                {
                    if(listener is IFeatureName li)
                    {
                        
                    }

                    await listener.HandleListener(r, manager);
                }
            });
        }

        public static void Register(IServiceCollection services)
        {
            services.AddFeatureManagement();
            services.AddSingleton<IListener<TRequest>, TSelf>();
            services.AddSingleton<Dispatch>(provider => request =>
                Dispatch(
                    request,
                    provider.GetServices<IListener<TRequest>>().ToList(),
                    provider.GetRequiredService<IFeatureManager>(),
                    provider.GetServices<IPipeline>().ToList()));
        }
    }
}