using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.FeatureManagement;

namespace FeatureSlice;

public static class ServiceCollectionExtensions
{
    public static void AddFeature<TService, TImplementation>(this IServiceCollection services)
        where TService : class, IFeatureSliceBase
        where TImplementation : class, TService
    {
        services.AddFeatureManagement();
        
        TService.RegsiterFeature<TService, TImplementation>(services);
    }
    
    public static void AddFeaturePipeline<TImplementation>(this IServiceCollection services)
        where TImplementation : class, IFeatureSlicePipelineBase<TImplementation>
    {
        TImplementation.RegisterPipeline<TImplementation>(services);
    }

    public static string GetFeatureName<TFeature>(this TFeature feature)
        where TFeature : IFeatureToggle
    {
        return TFeature.FeatureName;
    }
}