using FeatureSliceGenerator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using OneOf;
using OneOf.Types;

namespace FeatureSlice.New.Generation;

//------------ Base
public interface IMethodBase
{
}

public interface IMethodBase<TRequest, TResponse> : IMethodBase
{
    public TResponse Handle(TRequest request);
}

public interface IMethodPipeline<TRequest, TResponse>
{
    public TResponse Handle(TRequest request, Func<TRequest, TResponse> next);
}




//------------ Dispatcher

public interface IDispatcher<T>
    where T : IMethodBase
{
    internal T Method { get; }

    internal IServiceProvider Provider { get; }
}

internal sealed class Dispatcher<T> : IDispatcher<T>
    where T : IMethodBase
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

[GenerateExtension("IMethod")]
public interface IMethod<TRequest, TResponse> : IMethodBase<TRequest, TResponse>
{
    [GenerateExtensionMethod("Microsoft.Extensions.DependencyInjection")]
    public static void AddFeature<TService, TImplementation>(IServiceCollection services)
        where TService : class, IMethod<TRequest, TResponse>
        where TImplementation : class, TService
    {
        var key = "implementation";

        services.AddKeyedSingleton<TImplementation>(key);

        services.AddSingleton<IDispatcher<TService>>(provider => {
            var method = provider.GetRequiredKeyedService<TImplementation>(key);
            return new Dispatcher<TService>(method, provider);
        });
    }

    [GenerateExtensionMethod]
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

//------------ FeatureSlice

public struct Disabled();

[GenerateExtension("IFeatureSlice")]
public interface IFeatureSlice<TRequest, TResponse> : IMethodBase<TRequest, Task<TResponse>>, IFeatureName
{
    [GenerateExtensionMethod("Microsoft.Extensions.DependencyInjection")]
    public static void AddFeature<TService, TImplementation>(IServiceCollection services)
        where TService : class, IFeatureSlice<TRequest, TResponse>
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

    [GenerateExtensionMethod]
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

public interface IMessageConsumerSetup
{
    internal Task Setup(IMessagingConfiguration configuration);
}

[GenerateExtension("IMessageConsumer")]
public interface IMessageConsumer<TMessage> : IMethodBase<TMessage, Task<OneOf<Success, Retry, Error>>>, IMessageConsumerSetup, IFeatureName
    where TMessage : IMessage
{
    [GenerateExtensionMethod("Microsoft.Extensions.DependencyInjection")]
    public static void AddFeature<TService, TImplementation>(IServiceCollection services)
        where TService : class, IMessageConsumer<TMessage>
        where TImplementation : class, TService
    {
        var key = "implementation";

        services.AddKeyedSingleton<TImplementation>(key);
        services.AddFeatureManagement();
        services.AddSingleton<IMessageConsumerSetup, TImplementation>();

        services.AddSingleton<IDispatcher<TService>>(provider => {
            var method = provider.GetRequiredKeyedService<TImplementation>(key);
            return new Dispatcher<TService>(method, provider);
        });
    }

    [GenerateExtensionMethod]
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

    Task IMessageConsumerSetup.Setup(IMessagingConfiguration configuration)
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

//     public static void AddFeature<TService, TImplementation>(this IServiceCollection services)
//         where TService : class, IExampleMessageConsumer
//         where TImplementation : class, TService
//     {
//         IExampleMessageConsumer.AddFeature<TService, TImplementation>(services);
//     }
// }