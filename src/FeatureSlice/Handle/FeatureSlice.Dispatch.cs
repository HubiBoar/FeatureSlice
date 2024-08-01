using Microsoft.Extensions.DependencyInjection;
using Definit.Results;
using Microsoft.Extensions.Hosting;

namespace FeatureSlice;

public delegate Task<TResult> Handle<TRequest, TResult, TResponse>(TRequest request)
    where TRequest : notnull
    where TResult : Result_Base<TResponse>
    where TResponse : notnull;

public delegate Handle<TRequest, TResult, TResponse> DisptacherFactory<TRequest, TResult, TResponse>
(
    IServiceProvider provider,
    Handle<TRequest, TResult, TResponse> handle
)
    where TRequest : notnull
    where TResult : Result_Base<TResponse>
    where TResponse : notnull;

public interface IDispatcher
{
    public Handle<TRequest, TResult, TResponse> GetDispatcher<TRequest, TResult, TResponse>
    (
        IServiceProvider provider,
        Handle<TRequest, TResult, TResponse> dispatch
    )
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull;

    public sealed class Default : IDispatcher
    {
        public Handle<TRequest, TResult, TResponse> GetDispatcher<TRequest, TResult, TResponse>
        (
            IServiceProvider provider,
            Handle<TRequest, TResult, TResponse> dispatch
        )
            where TRequest : notnull
            where TResult : Result_Base<TResponse>
            where TResponse : notnull
        {
            return dispatch;
        }
    }

    public static ServiceDescriptor RegisterDefault()
    {
        return new ServiceDescriptor(typeof(IDispatcher), typeof(Default), ServiceLifetime.Singleton);
    }
}

public sealed record FeatureSliceOptions(IServiceCollection Services);

public static class DispatcherExtensions
{
    private sealed record Extension<THost>(Func<THost, IServiceProvider, Task> Run)
        where THost : IHost;

    public static FeatureSliceOptions AddFeatureSlices(this IServiceCollection services)
    {
        return new FeatureSliceOptions(services);
    }

    public static void AddFeatureSlicesExtension<THost>(this IServiceCollection services, Func<THost, IServiceProvider, Task> extension)
        where THost : IHost
    {
        services.AddSingleton(new Extension<THost>(extension));
    }

    public static async Task MapFeatureSlices<T>(this T host)
        where T : IHost
    {
        await using var scope = host.Services.CreateAsyncScope();

        var provider = scope.ServiceProvider;

        await Task.WhenAll
        (
            provider
                .GetServices<Extension<T>>()
                .Select(x => x.Run(host, provider))
                .ToArray()
        );
        
        if(typeof(T) == typeof(IHost))
        {
            return;
        }
        
        await Task.WhenAll
        (
            provider
                .GetServices<Extension<IHost>>()
                .Select(x => x.Run(host, provider))
                .ToArray()
        ); 
    }

    public static FeatureSliceOptions DefaultDispatcher(this FeatureSliceOptions options)
    {
        options.Services.Add(IDispatcher.RegisterDefault());

        return options;
    }
}