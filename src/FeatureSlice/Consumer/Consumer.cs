using Definit.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FeatureSlice;

public sealed record ConsumerName(string Name);

public interface IConsumerDispatcher
{
    public Dispatch<TRequest, Result, Success> GetDispatcher<TRequest>
    (
        ConsumerName consumerName,
        IServiceProvider provider,
        Dispatch<TRequest, Result, Success> dispatch
    )
        where TRequest : notnull;

    public sealed class Default : IConsumerDispatcher
    {
        public Dispatch<TRequest, Result, Success> GetDispatcher<TRequest>
        (
            ConsumerName consumerName,
            IServiceProvider provider,
            Dispatch<TRequest, Result, Success> dispatch
        )
            where TRequest : notnull
        {
            return async request =>
            {
                try
                {
                    return await dispatch(request);
                }
                catch(Exception exception)
                {
                    return exception;
                }            
            };
        }
    }
   
    public static ServiceDescriptor RegisterDefault()
    {
        return new ServiceDescriptor(typeof(IConsumerDispatcher), typeof(Default), ServiceLifetime.Singleton);
    }
}

public static class FeatureSliceConsumerExtensions
{
    public static FeatureSliceBase<TRequest, Result, Success>.ISetup AsConsumer<TRequest>
    (
        this FeatureSliceBase<TRequest, Result, Success>.ISetup options,
        ConsumerName consumerName
    )
        where TRequest : notnull
    {
        options.Extend(services => services.TryAdd(IConsumerDispatcher.RegisterDefault()));
        options.DispatcherFactory = 
            provider => 
                (prv, dispatch) => prv
                    .GetRequiredService<IConsumerDispatcher>()
                    .GetDispatcher(consumerName, prv, dispatch);
        
        return options;
    }

    public static FeatureSliceBase<TRequest, Result, Success>.ISetup AsConsumer<TRequest>
    (
        this FeatureSliceBase<TRequest, Result, Success>.ISetup options,
        string consumerName
    )
        where TRequest : notnull
    {
        return options.AsConsumer(new ConsumerName(consumerName));
    }

    public static FeatureSliceOptions DefaultConsumerDispatcher(this FeatureSliceOptions options)
    {
        options.Services.Add(IConsumerDispatcher.RegisterDefault());

        return options;
    }
}