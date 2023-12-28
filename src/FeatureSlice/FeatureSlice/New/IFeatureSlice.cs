using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using OneOf;

namespace FeatureSlice.New;

public interface IMethod<TInput, TOutput>
{
    protected TOutput Handle(TInput input);
}

public interface IFeatureSlice<TRequest, TResponse> : IMethod<TRequest, Task<TResponse>>
{
    public Task<TResponse> Send(TRequest input)
    {
        return Handle(input);
    }

    public static void Register<TService, TImplementation>(IServiceCollection services)
        where TService : class, IFeatureSlice<TRequest, TResponse>
        where TImplementation : class, TService
    {
        services.AddSingleton<TService, TImplementation>();
    }
}

public sealed record Disabled;

public interface IFeatureDispatcher<TRequest, TResponse>
{
    internal Task<OneOf<TResponse, Disabled>> Send(TRequest input, IFeatureSliceToggle<TRequest, TResponse> slice);
}

internal class FeatureSliceToggleDispatcher<TRequest, TResponse> : IFeatureDispatcher<TRequest, TResponse>
{
    private readonly IFeatureManager _featureManager;

    public FeatureSliceToggleDispatcher(IFeatureManager featureManager)
    {
        _featureManager = featureManager;
    }

    async Task<OneOf<TResponse, Disabled>> IFeatureDispatcher<TRequest, TResponse>.Send(TRequest input, IFeatureSliceToggle<TRequest, TResponse> slice)
    {
        var enabled = await _featureManager.IsEnabledAsync(slice.FeatureName);

        if (enabled == false)
        {
            return new Disabled();
        }

        return await slice.InternalSliceHandle(input);
    }
}

public interface IFeatureSliceToggle<TRequest, TResponse> : IMethod<TRequest, Task<TResponse>>
{
    internal abstract string FeatureName { get; }

    protected IFeatureDispatcher<TRequest, TResponse> Dispatcher { get; }

    public Task<OneOf<TResponse, Disabled>> Send(TRequest input)
    {
        return Dispatcher.Send(input, this);
    }

    internal Task<TResponse> InternalSliceHandle(TRequest input)
    {
        return this.Handle(input);
    }

    public static void Register<TService, TImplementation>(IServiceCollection services)
        where TService : class, IFeatureSliceToggle<TRequest, TResponse>
        where TImplementation : class, TService
    {
        services.AddFeatureManagement();
        services.AddSingleton<TService, TImplementation>();
        services.AddSingleton<IFeatureDispatcher<TRequest, TResponse>, FeatureSliceToggleDispatcher<TRequest, TResponse>>();
    }
}











