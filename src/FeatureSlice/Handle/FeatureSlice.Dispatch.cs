using Microsoft.Extensions.DependencyInjection;
using Definit.Results;

namespace FeatureSlice;

public delegate Task<TResult> Dispatch<TRequest, TResult, TResponse>(TRequest request)
    where TRequest : notnull
    where TResult : Result_Base<TResponse>
    where TResponse : notnull;

public delegate Dispatch<TRequest, TResult, TResponse> Dispatcher<TRequest, TResult, TResponse>
(
    IServiceProvider provider,
    Dispatch<TRequest, TResult, TResponse> dispatch
)
    where TRequest : notnull
    where TResult : Result_Base<TResponse>
    where TResponse : notnull;

public interface IDispatcher
{
    public Dispatch<TRequest, TResult, TResponse> GetDispatcher<TRequest, TResult, TResponse>
    (
        IServiceProvider provider,
        Dispatch<TRequest, TResult, TResponse> dispatch
    )
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull;

    public sealed class Default : IDispatcher
    {
        public Dispatch<TRequest, TResult, TResponse> GetDispatcher<TRequest, TResult, TResponse>
        (
            IServiceProvider provider,
            Dispatch<TRequest, TResult, TResponse> dispatch
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
    public static FeatureSliceOptions AddFeatureSlices(this IServiceCollection services)
    {
        return new FeatureSliceOptions(services);
    }

    public static FeatureSliceOptions DefaultDispatcher(this FeatureSliceOptions options)
    {
        options.Services.Add(IDispatcher.RegisterDefault());

        return options;
    }
}