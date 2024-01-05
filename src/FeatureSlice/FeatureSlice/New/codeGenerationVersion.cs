using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using OneOf;
using OneOf.Types;

namespace FeatureSlice.New.Generation;

//------------ Base

public interface IMethod
{
}

public interface IMethodBase<TRequest, TResponse> : IMethod
{
    public TResponse Handle(TRequest request);
}

public interface IMethodPipeline<TRequest, TResponse>
{
    public TResponse Handle(TRequest request, Func<TRequest, TResponse> next);
}




//------------ Dispatcher

public interface IDispatcher<T>
    where T : IMethod
{
    internal T Method { get; }

    internal IServiceProvider Provider { get; }
}

internal sealed class Dispatcher<T> : IDispatcher<T>
    where T : IMethod
{
    public T Method { get; }

    public IServiceProvider Provider { get; }

    public Dispatcher(T method, IServiceProvider provider)
    {
        Method = method;
        Provider = provider;
    }
}



//------------ Method

public interface IMethodRegisterer : IMethod
{
    public static void AddFeature<TService, TImplementation>(IServiceCollection services)
        where TService : class, IMethodRegisterer
        where TImplementation : class, TService
    {
        var key = "implementation";

        services.AddKeyedSingleton<TImplementation>(key);

        services.AddSingleton<IDispatcher<TService>>(provider => {
            var method = provider.GetRequiredKeyedService<TImplementation>(key);
            return new Dispatcher<TService>(method, provider);
        });
    }
}

public interface IMethod<TRequest, TResponse> : IMethodBase<TRequest, TResponse>, IMethodRegisterer
{
    public static TResponse Send<T>(IDispatcher<T> dispatcher, TRequest request)
        where T : IMethod<TRequest, TResponse>
    {
        var pipelines = dispatcher.Provider.GetServices<IMethodPipeline<TRequest, TResponse>>();

        return DispatcherHelper.RunPipeline(request, dispatcher.Method.Handle, 0, pipelines.ToList());
    }
}

public interface IExampleMethod : IMethod<IExampleMethod.Request, IExampleMethod.Response>
{
    public sealed record Request();
    public sealed record Response();
}

public static class MethodRegisterer
{
    public static void AddFeature<TService, TImplementation>(this IServiceCollection services)
        where TService : class, IMethodRegisterer
        where TImplementation : class, TService
    {
        IMethodRegisterer.AddFeature<TService, TImplementation>(services);
    }
}



//------------ FeatureSlice

public interface IFeatureSliceRegisterer : IMethod
{
    public static void AddFeature<TService, TImplementation>(IServiceCollection services)
        where TService : class, IFeatureSliceRegisterer
        where TImplementation : class, TService
    {
        var key = "implementation";

        services.AddKeyedSingleton<TImplementation>(key);
        services.AddFeatureManagement();

        services.AddSingleton<IDispatcher<TService>>(provider => {
            var method = provider.GetRequiredKeyedService<TImplementation>(key);
            return new Dispatcher<TService>(method, provider);
        });
    }
}

public struct Disabled();

public interface IFeatureSlice<TRequest, TResponse> : IMethodBase<TRequest, Task<TResponse>>, IFeatureName, IFeatureSliceRegisterer
{
    public static async Task<OneOf<TResponse, Disabled>> Send<T>(IDispatcher<T> dispatcher, TRequest request)
        where T : IFeatureSlice<TRequest, TResponse>
    {
        var pipelines = dispatcher.Provider.GetServices<IMethodPipeline<TRequest, Task<TResponse>>>();

        var manager = dispatcher.Provider.GetRequiredService<IFeatureManager>();

        if(await manager.IsEnabledAsync(dispatcher.Method))
        {
            return new Disabled();
        }

        return await DispatcherHelper.RunPipeline(request, dispatcher.Method.Handle, 0, pipelines.ToList());
    }
}

public interface IFeatureName
{
    public abstract string FeatureName { get; }
}

public interface IExampleFeatureSlice : IFeatureSlice<IExampleFeatureSlice.Request, IExampleFeatureSlice.Response>
{
    public sealed record Request();
    public sealed record Response();
}

public static class FeatureRegisterer
{
    public static void AddFeature<TService, TImplementation>(this IServiceCollection services)
        where TService : class, IFeatureSliceRegisterer
        where TImplementation : class, TService
    {
        IFeatureSliceRegisterer.AddFeature<TService, TImplementation>(services);
    }
}



//------------ MessageConsumer


public interface IMessage
{
    public static abstract string MessageName { get; }
}

public struct Retry();

public interface IMessagingConfiguration
{
    public Task Register<TMessage>(IMessageConsumer<TMessage> consumer)
        where TMessage : IMessage;

    public Task Send<TMessage>(TMessage request, IMessageConsumer<TMessage> consumer)
        where TMessage : IMessage;
}

public interface IMessageConsumerRegisterer : IMethod
{
    public static void AddFeature<TService, TImplementation>(IServiceCollection services)
        where TService : class, IMessageConsumerRegisterer
        where TImplementation : class, TService
    {
        var key = "implementation";

        services.AddKeyedSingleton<TImplementation>(key);
        services.AddFeatureManagement();
        services.AddSingleton<IMessageConsumerRegisterer, TImplementation>();

        services.AddSingleton<IDispatcher<TService>>(provider => {
            var method = provider.GetRequiredKeyedService<TImplementation>(key);
            return new Dispatcher<TService>(method, provider);
        });
    }

    internal Task Setup(IMessagingConfiguration configuration);
}

public interface IMessageConsumer<TMessage> : IMethodBase<TMessage, Task<OneOf<Success, Retry, Error>>>, IMessageConsumerRegisterer, IFeatureName
    where TMessage : IMessage
{
    public static async Task<OneOf<Success, Disabled>> Send<T>(IDispatcher<T> dispatcher, TMessage request)
        where T : IMessageConsumer<TMessage>
    {
        var manager = dispatcher.Provider.GetRequiredService<IFeatureManager>();

        if(await manager.IsEnabledAsync(dispatcher.Method))
        {
            return new Disabled();
        }

        var messagingConfiguration = dispatcher.Provider.GetRequiredService<IMessagingConfiguration>();

        await messagingConfiguration.Send(request, dispatcher.Method);

        return new Success();
    }

    Task IMessageConsumerRegisterer.Setup(IMessagingConfiguration configuration)
    {
        return configuration.Register(this);
    }
}


public interface IExampleMessageConsumer : IMessageConsumer<IExampleMessageConsumer.Message>
{
    public sealed record Message() : IMessage
    {
        public static string MessageName => "ExampleMessage";
    }
}

public static class MessageConsumerRegisterer
{
    public static void AddFeature<TService, TImplementation>(this IServiceCollection services)
        where TService : class, IMessageConsumerRegisterer
        where TImplementation : class, TService
    {
        IMessageConsumerRegisterer.AddFeature<TService, TImplementation>(services);
    }
}




public static class DispatcherHelper
{
    public static TResponse RunPipeline<TRequest, TResponse>(
        TRequest request,
        Func<TRequest, TResponse> featureMethod,
        int index,
        IReadOnlyList<IMethodPipeline<TRequest, TResponse>> pipelines)
    {
        if (index < pipelines.Count)
        {
            return pipelines[index].Handle(request, r => RunPipeline(r, featureMethod, index++, pipelines));
        }
        else
        {
            return featureMethod.Invoke(request);
        }
    }

    public static Task<bool> IsEnabledAsync(this IFeatureManager featureManager, IFeatureName featureName)
    {
        return featureManager.IsEnabledAsync(featureName.FeatureName);
    }
}




//------------ Generate Automatically
// internal static class DisaptcherExtensions
// {
//     public static IExampleMethod.Response Send<T>(this IDispatcher<T> dispatcher, IExampleMethod.Request request)
//         where T : IMethod<IExampleMethod.Request, IExampleMethod.Response>
//     {
//         return IMethod<IExampleMethod.Request, IExampleMethod.Response>.Send(dispatcher, request);
//     }

//     public static Task<OneOf<IExampleFeatureSlice.Response, Disabled>> Send<T>(this IDispatcher<T> dispatcher, IExampleFeatureSlice.Request request)
//         where T : IExampleFeatureSlice
//     {
//         return IExampleFeatureSlice.Send(dispatcher, request);
//     }

//     public static Task<OneOf<Success, Disabled>> Send<T>(this IDispatcher<T> dispatcher, IExampleMessageConsumer.Message message)
//         where T : IExampleMessageConsumer
//     {
//         return IExampleMessageConsumer.Send(dispatcher, message);
//     }
// }