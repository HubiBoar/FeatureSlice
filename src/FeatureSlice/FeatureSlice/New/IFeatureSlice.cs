using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using OneOf;

namespace FeatureSlice.New;


public interface IFeatureDispatcher
{
    internal Task<TResponse> Send<TRequest, TResponse>(TRequest input, IFeatureSlice<TRequest, TResponse>.Delegate method);
}

public interface IMethod<TInput, TOutput>
{
    internal TOutput Handle(TInput input);
}

public interface IFeatureSlice<TRequest, TResponse> : IMethod<TRequest, Task<TResponse>>
{
    public delegate Task<TResponse> Delegate(TRequest request);

    protected IFeatureDispatcher? Dispatcher { get; set; }

    public Task<TResponse> Send(TRequest input)
    {
        return Dispatcher!.Send(input, this.Handle);
    }

    public static void Register<TService, TImplementation>(IServiceCollection services)
        where TService : class, IFeatureSlice<TRequest, TResponse>
        where TImplementation : class, TService
    {
        services.AddSingleton<TImplementation>();

        services.AddSingleton<TService>(provider => {
            var implementation = provider.GetRequiredService<TImplementation>();

            implementation.Dispatcher = new FeatureSliceDispatcher();

            return implementation;
        });
    }
}

public interface IFeatureDispatcher<TRequest, TResponse>
{
    internal Task<TResponse> Send(TRequest input, IFeatureSlice<TRequest, TResponse> slice);
}

internal class FeatureSliceDispatcher : IFeatureDispatcher
{
    public Task<TResponse> Send<TRequest, TResponse>(TRequest input, IFeatureSlice<TRequest, TResponse>.Delegate method)
    {
        return method(input);
    }
}




public sealed record Disabled;

public interface IFeatureToggleDispatcher<TRequest, TResponse>
{
    internal Task<OneOf<TResponse, Disabled>> Send(TRequest input, IFeatureSliceToggle<TRequest, TResponse> slice);
}

internal class FeatureSliceToggleDispatcher<TRequest, TResponse> : IFeatureToggleDispatcher<TRequest, TResponse>
{
    private readonly IFeatureManager _featureManager;
    private readonly string _featureName;

    public FeatureSliceToggleDispatcher(IFeatureManager featureManager, string featureName)
    {
        _featureName = featureName;
        _featureManager = featureManager;
    }

    async Task<OneOf<TResponse, Disabled>> IFeatureToggleDispatcher<TRequest, TResponse>.Send(TRequest input, IFeatureSliceToggle<TRequest, TResponse> slice)
    {
        var enabled = await _featureManager.IsEnabledAsync(_featureName);

        if (enabled == false)
        {
            return new Disabled();
        }

        return await slice.InternalHandle(input);
    }
}

public interface IFeatureSliceToggle<TRequest, TResponse> : IMethod<TRequest, Task<TResponse>>
{
    public static abstract string FeatureName { get; }

    protected IFeatureToggleDispatcher<TRequest, TResponse> Dispatcher { get; set; }

    public Task<OneOf<TResponse, Disabled>> Send(TRequest input)
    {
        return Dispatcher.Send(input, this);
    }

    internal Task<TResponse> InternalHandle(TRequest input)
    {
        return Handle(input);
    }

    public static void Register<TService, TImplementation>(IServiceCollection services)
        where TService : class, IFeatureSliceToggle<TRequest, TResponse>
        where TImplementation : class, TService
    {
        services.AddFeatureManagement();
        services.AddSingleton<TImplementation>();

        services.AddSingleton<TService>(provider => {
            var implementation = provider.GetRequiredService<TImplementation>();
            var manager = provider.GetRequiredService<IFeatureManager>();

            implementation.Dispatcher = new FeatureSliceToggleDispatcher<TRequest, TResponse>(manager, TImplementation.FeatureName);

            return implementation;
        });
    }
}