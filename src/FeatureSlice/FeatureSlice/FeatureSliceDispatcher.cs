using Microsoft.FeatureManagement;
using OneOf;

namespace FeatureSlice;

public struct Disabled;

public interface IDispatcher<TRequest, TResponse>
{
    internal Task<OneOf<TResponse, Disabled>> Send(Func<TRequest, Task<TResponse>> featureMethod, TRequest request);
}

internal sealed class FeatureSliceDispatcher<TRequest, TResponse, TFeatureName>
    : IDispatcher<TRequest, TResponse>
    where TFeatureName : IFeatureToggle
{
    private readonly IFeatureManager _featureManager;
    private readonly IFeatureSlicePipeline<TRequest, TResponse>[] _pipelines;

    public FeatureSliceDispatcher(
        IFeatureManager featureManager,
        IEnumerable<IFeatureSlicePipeline<TRequest, TResponse>> pipelines)
    {
        _featureManager = featureManager;
        _pipelines = pipelines.ToArray();
    }
    
    public async Task<OneOf<TResponse, Disabled>> Send(Func<TRequest, Task<TResponse>> featureMethod, TRequest request)
    {
        var enabled = await _featureManager.IsEnabledAsync(TFeatureName.FeatureName);

        if (enabled == false)
        {
            return new Disabled();
        }

        return await Handle(featureMethod, 0, request);
    }

    private Task<TResponse> Handle(Func<TRequest, Task<TResponse>> featureMethod, int index, TRequest request)
    {
        if (index < _pipelines.Length)
        {
            return _pipelines[index].Handle(request, r => Handle(featureMethod, index++, r));
        }
        else
        {
            return featureMethod.Invoke(request);
        }
    }
}