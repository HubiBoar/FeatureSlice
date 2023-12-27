using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.FeatureManagement;
using OneOf;

namespace FeatureSlice;

public interface IFeatureToggle
{
    public static abstract string FeatureName { get; }
}

public interface IFeatureSliceBase : IFeatureToggle
{
    public static abstract void RegsiterFeature<TService, TImplementation>(IServiceCollection services)
        where TService : class, IFeatureSliceBase
        where TImplementation : class, TService;
}

public interface IFeatureSlice<TRequest, TResponse> : IFeatureSliceBase
{
    internal IDispatcher<TRequest, TResponse> Dispatcher { get; }

    protected Task<TResponse> Handle(TRequest request);

    static void IFeatureSliceBase.RegsiterFeature<TService, TImplementation>(IServiceCollection services)
    {
        services.AddFeatureManagement();
        services.AddSingleton<IDispatcher<TRequest, TResponse>, FeatureSliceDispatcher<TRequest, TResponse, TImplementation>>();
        services.TryAddTransient<TService, TImplementation>();
    }
    
    public async Task<OneOf<TResponse, Disabled, Exception>> Send(TRequest request)
    {
        try
        {
            var response = await Dispatcher.Send(Handle, request);

            return response.Match<OneOf<TResponse, Disabled, Exception>>(r => r, d => d);
        }
        catch (Exception exception)
        {
            return exception;
        } 
    }
}