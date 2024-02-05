using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using OneOf;
using OneOf.Types;

namespace FeatureSlice;

public struct Disabled;

public interface IFeatureSlice
{

}


public interface IHandler<TRequest, TResponse> : IMethod<TRequest, Task<OneOf<TResponse, Error>>>
{   
    public abstract static string Name { get; }

    public interface IRegistrable<TSelf, TDispatcher> : IHandler<TRequest, TResponse>
        where TSelf : class, IRegistrable<TSelf, TDispatcher>
        where TDispatcher : Delegate
    {
        public delegate Task<OneOf<TResponse, Disabled, Error>> Dispatch(TRequest request);

        public static void Register(IServiceCollection services)
        {
            services.AddFeatureManagement();
            services.AddSingleton<TSelf>();
            services.AddSingleton<TDispatcher>(provider => TSelf.Convert(request => DispatchMethod(request, provider)));
            services.AddSingleton<Publisher<TRequest>.Listen>(RegisterListener);

            Publisher<TRequest>.Listen RegisterListener(IServiceProvider provider)
            {
                return async request => {
                    var result = await DispatchMethod(request, provider);
                    return result.Match<OneOf<Success, Error>>(success => new Success(), disabled => new Success(), errror => errror);
                };
            }
        }

        public static Task<OneOf<TResponse, Disabled, Error>> DispatchMethod(
            TRequest request,
            IServiceProvider provider)
        {
            return DispatchMethod(
                request,
                provider.GetRequiredService<TSelf>(),
                provider.GetRequiredService<IFeatureManager>(),
                provider.GetServices<IPipeline>().ToList());
        }

        public static async Task<OneOf<TResponse, Disabled, Error>> DispatchMethod(
            TRequest request,
            TSelf self,
            IFeatureManager featureManager,
            IReadOnlyList<IPipeline> pipelines)
        {
            var isEnabled = await featureManager.IsEnabledAsync(TSelf.Name);
            if(isEnabled == false)
            {
                return new Disabled();
            }

            var pipelinesResult = await pipelines.RunPipeline(request, self.Handle);
            return pipelinesResult.Match<OneOf<TResponse, Disabled, Error>>(success => success, error => error);
        }


        public static abstract TDispatcher Convert(Dispatch dispatch);
    }
}