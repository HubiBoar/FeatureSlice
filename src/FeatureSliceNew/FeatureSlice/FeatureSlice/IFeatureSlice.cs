using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using OneOf;
using OneOf.Types;

namespace FeatureSlice;

public struct Disabled();

public static partial class Feature
{
    public interface IHandler<TRequest, TResponse> : IMethod<TRequest, Task<OneOf<TResponse, Error>>>
    {
        public abstract static string Name { get; }

        public static class Setup<TSelf>
            where TSelf : class, IHandler<TRequest, TResponse>
        {
            public static async Task<OneOf<TResponse, Disabled, Error>> Dispatch(TRequest request, TSelf self, IFeatureManager featureManager, IReadOnlyList<IPipeline> pipelines)
            {
                if(await featureManager.IsEnabledAsync(TSelf.Name))
                {
                    return new Disabled();
                }

                return (await pipelines.RunPipeline(request, self.Handle)).Match<OneOf<TResponse, Disabled, Error>>(r => r, e => e);
            }

            public static Func<TRequest, Task<OneOf<TResponse, Disabled, Error>>> DispatchFactory(IServiceProvider provider)
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
                services.AddSingleton(RegisterListener);

                static Publisher<TRequest>.Listen RegisterListener(IServiceProvider provider)
                {
                    return async request => (await DispatchFactory(provider)(request)).Match<OneOf<Success, Error>>(s => new Success(), r => new Success(), e => e);
                }
            }
        }
    }
}