using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using OneOf;

namespace FeatureSlice;

public struct Disabled();

public partial interface IFeatureSlice<TRequest, TResponse>
{
    public interface WithToggle : IMethod<TRequest, Task<TResponse>>, IFeatureName
    {
        public static class Setup<TSelf>
            where TSelf : class, WithToggle
        {
            public static async Task<OneOf<TResponse, Disabled>> Dispatch(TRequest request, TSelf self, IFeatureManager featureManager, IReadOnlyList<IPipeline> pipelines)
            {
                if(await featureManager.IsEnabledAsync<TSelf>())
                {
                    return new Disabled();    
                }

                return await pipelines.RunPipeline(request, self.Handle);
            }

            public static Func<TRequest, Task<OneOf<TResponse, Disabled>>> Factory(IServiceProvider provider)
            {
                return request => Dispatch(
                    request,
                    provider.GetRequiredService<TSelf>(),
                    provider.GetRequiredService<IFeatureManager>(),
                    provider.GetServices<IPipeline>().ToList());
            }

            public static void Register<TDispatcher>(IServiceCollection services, Func<IServiceProvider, TDispatcher> dispatcherFactory)
                where TDispatcher : Delegate
            {
                services.AddFeatureManagement();
                services.AddSingleton<TSelf>();
                services.AddSingleton<TDispatcher>(dispatcherFactory);
            }
        }
    }
}