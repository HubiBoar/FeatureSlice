using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using OneOf;

namespace FeatureSlice.Dispatch;

public interface IFeatureSlice<TRequest, TResponse> : IMethod<TRequest, Task<TResponse>>
{
}

public abstract class FeatureSlice<TSelf, TRequest, TResponse> : IFeatureSlice<TRequest, TResponse>, IRegistrable
    where TSelf : FeatureSlice<TSelf, TRequest, TResponse>, IFeatureName
{
    public abstract Task<TResponse> Handle(TRequest request);

    public delegate Task<OneOf<TResponse, Disabled>> Dispatcher(TRequest request);

    public static async Task<OneOf<TResponse, Disabled>> Dispatch(TRequest request, TSelf self, IFeatureManager featureManager, IReadOnlyList<IFeatureSlice<TRequest, TResponse>.IPipeline> pipelines)
    {
        if(await featureManager.IsEnabledAsync<TSelf>())
        {
            return new Disabled();    
        }

        return await pipelines.RunPipeline(request, self.Handle);
    }

    public static void Register(IApplicationSetup setup)
    {
        setup.Services.AddFeatureManagement();
        setup.Services.AddSingleton<TSelf>();
        setup.Services.AddSingleton<Dispatcher>(provider => request => Dispatch(
            request,
            provider.GetRequiredService<TSelf>(),
            provider.GetRequiredService<IFeatureManager>(),
            provider.GetServices<IFeatureSlice<TRequest, TResponse>.IPipeline>().ToList())
        );
    }
}