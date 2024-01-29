using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using OneOf;
using OneOf.Types;

namespace FeatureSlice;

public struct Disabled();

public sealed partial class FeatureSlice<TRequest, TResponse>
{
    public partial interface IHandler : IMethod<TRequest, Task<OneOf<TResponse, Error>>>, FeatureSlice<TRequest>.IListener
    {
        public abstract static string FeatureName { get; }

        public static class Setup<TSelf>
            where TSelf : class, IHandler
        {
            public static async Task<OneOf<TResponse, Disabled, Error>> Dispatch(TRequest request, TSelf self, IFeatureManager featureManager, IReadOnlyList<IPipeline> pipelines)
            {
                if(await featureManager.IsEnabledAsync(TSelf.FeatureName))
                {
                    return new Disabled();    
                }

                return (await pipelines.RunPipeline(request, self.Handle)).Match<OneOf<TResponse, Disabled, Error>>(r => r, e => e);
            }

            public static Func<TRequest, Task<OneOf<TResponse, Disabled, Error>>> Factory(IServiceProvider provider)
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
                //Listener
                services.AddSingleton<FeatureSlice<TRequest>.IListener, TSelf>();
                services.Register<FeatureSlice<TRequest>>();

                //Handler
                services.AddFeatureManagement();
                services.AddSingleton<TSelf>();
                services.AddSingleton<TDispatcher>(dispatcherFactory);
            }
        }

        async Task<OneOf<Success, Error>> FeatureSlice<TRequest>.IListener.Listen(TRequest request)
        {
            return (await Handle(request)).Match<OneOf<Success, Error>>(_ => new Success(), e => e);
        }
    }
}