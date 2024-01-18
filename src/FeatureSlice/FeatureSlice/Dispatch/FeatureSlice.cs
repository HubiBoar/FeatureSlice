using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
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

    public interface IDispatcher
    {
        public Task<OneOf<TResponse, Disabled>> Send(TRequest request);
    }

    private sealed class Dispatcher : IDispatcher
    {
        private readonly TSelf _self;
        private readonly IFeatureManager _featureManager;
        private readonly IReadOnlyList<IFeatureSlice<TRequest, TResponse>.IPipeline> _pipelines;

        public Dispatcher(TSelf self, IFeatureManager featureManager, IEnumerable<IFeatureSlice<TRequest, TResponse>.IPipeline> pipelines)
        {
            _self = self;
            _featureManager = featureManager;
            _pipelines = pipelines.ToList();
        }

        public async Task<OneOf<TResponse, Disabled>> Send(TRequest request)
        {
            if(await _featureManager.IsEnabledAsync<TSelf>())
            {
                return new Disabled();    
            }

            return await _pipelines.RunPipeline(request, _self.Handle);
        }
    }

    public static void Register(IServiceCollection services)
    {
        services.AddFeatureManagement();
        services.AddSingleton<TSelf>();
        services.AddSingleton<IDispatcher, Dispatcher>();
    }
}