using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using OneOf;
using OneOf.Types;



//------------ Base

internal interface IMethod
{
}

internal interface IMethodBase<TRequest, TResponse> : IMethod
{
    public TResponse Handle(TRequest request);
}

internal interface IMethodPipeline<TRequest, TResponse>
{
    public TResponse Handle(TRequest request, Func<TRequest, TResponse> next);
}




//------------ Dispatcher

internal interface IDispatcher<T>
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

internal interface IMethodRegisterer : IMethod
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

internal interface IMethod<TRequest, TResponse> : IMethodBase<TRequest, TResponse>, IMethodRegisterer
{
    public static TResponse Send<T>(IDispatcher<T> dispatcher, TRequest request)
        where T : IMethod<TRequest, TResponse>
    {
        var pipelines = dispatcher.Provider.GetServices<IMethodPipeline<TRequest, TResponse>>();

        return DispatcherHelper.RunPipeline(request, dispatcher.Method.Handle, 0, pipelines.ToList());
    }
}

internal interface IExampleMethod : IMethod<IExampleMethod.Request, IExampleMethod.Response>
{
    public sealed record Request();
    public sealed record Response();
}

internal sealed class ExampleMethod : IExampleMethod
{
    public IExampleMethod.Response Handle(IExampleMethod.Request request)
    {
        return new IExampleMethod.Response();
    }
}

internal static class MethodRegisterer
{
    public static void AddFeature<TService, TImplementation>(this IServiceCollection services)
        where TService : class, IMethodRegisterer
        where TImplementation : class, TService
    {
        IMethodRegisterer.AddFeature<TService, TImplementation>(services);
    }
}



//------------ FeatureSlice

internal interface IFeatureSliceRegisterer : IMethod
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

internal interface IFeatureSlice<TRequest, TResponse> : IMethodBase<TRequest, Task<TResponse>>, IFeatureName, IFeatureSliceRegisterer
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

internal interface IFeatureName
{
    public abstract string FeatureName { get; }
}

internal interface IExampleFeatureSlice : IFeatureSlice<IExampleFeatureSlice.Request, IExampleFeatureSlice.Response>
{
    public sealed record Request();
    public sealed record Response();
}

internal class ExampleFeatureSlice : IExampleFeatureSlice
{
    public string FeatureName => "ExampleFeatureSlice";

    public Task<IExampleFeatureSlice.Response> Handle(IExampleFeatureSlice.Request request)
    {
        return Task.FromResult(new IExampleFeatureSlice.Response());
    }
}

internal static class FeatureRegisterer
{
    public static void AddFeature<TService, TImplementation>(this IServiceCollection services)
        where TService : class, IFeatureSliceRegisterer
        where TImplementation : class, TService
    {
        IFeatureSliceRegisterer.AddFeature<TService, TImplementation>(services);
    }
}



//------------ MessageConsumer


internal interface IMessage
{
    public static abstract string MessageName { get; }
}

internal struct Retry();

internal interface IMessagingConfiguration
{
    public Task Register<TMessage>(IMessageConsumer<TMessage> consumer)
        where TMessage : IMessage;

    public Task Send<TMessage>(TMessage request, IMessageConsumer<TMessage> consumer)
        where TMessage : IMessage;
}

internal interface IMessageConsumerRegisterer : IMethod
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

internal interface IMessageConsumer<TMessage> : IMethodBase<TMessage, Task<OneOf<Success, Retry, Error>>>, IMessageConsumerRegisterer, IFeatureName
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


internal interface IExampleMessageConsumer : IMessageConsumer<IExampleMessageConsumer.Message>
{
    public sealed record Message() : IMessage
    {
        public static string MessageName => "ExampleMessage";
    }
}

internal sealed class ExampleMessageConsumer : IExampleMessageConsumer
{
    public string FeatureName => "ExampleMessageConsumer";

    public Task<OneOf<Success, Retry, Error>> Handle(IExampleMessageConsumer.Message request)
    {
        return Task.FromResult(OneOf<Success, Retry, Error>.FromT0(new Success()));
    }
}

internal static class MessageConsumerRegisterer
{
    public static void AddFeature<TService, TImplementation>(this IServiceCollection services)
        where TService : class, IMessageConsumerRegisterer
        where TImplementation : class, TService
    {
        IMessageConsumerRegisterer.AddFeature<TService, TImplementation>(services);
    }
}




//------------ Example

internal sealed class Dependncy
{
    public static void Register(IServiceCollection services)
    {
        services.AddFeature<IExampleFeatureSlice, ExampleFeatureSlice>();
        services.AddFeature<IExampleMethod, ExampleMethod>();
        services.AddFeature<IExampleMessageConsumer, ExampleMessageConsumer>();
    }

    private readonly IDispatcher<IExampleMethod> _method;
    private readonly IDispatcher<IExampleFeatureSlice> _featureSlice;
    private readonly IDispatcher<IExampleMessageConsumer> _consumer;

    public Dependncy(IDispatcher<IExampleMethod> method, IDispatcher<IExampleFeatureSlice> featureSlice, IDispatcher<IExampleMessageConsumer> consumer)
    {
        _method = method;
        _featureSlice = featureSlice;
        _consumer = consumer;
    }

    private async Task Run()
    {
        var methodResult = _method.Send(new IExampleMethod.Request());
        var featureResult = await _featureSlice.Send(new IExampleFeatureSlice.Request());
        var consumerResult = await _consumer.Send(new IExampleMessageConsumer.Message());
    }
}

internal static class DispatcherHelper
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
internal static class DisaptcherExtensions
{
    public static IExampleMethod.Response Send<T>(this IDispatcher<T> dispatcher, IExampleMethod.Request request)
        where T : IExampleMethod
    {
        return IExampleMethod.Send(dispatcher, request);
    }

    public static Task<OneOf<IExampleFeatureSlice.Response, Disabled>> Send<T>(this IDispatcher<T> dispatcher, IExampleFeatureSlice.Request request)
        where T : IExampleFeatureSlice
    {
        return IExampleFeatureSlice.Send(dispatcher, request);
    }

    public static Task<OneOf<Success, Disabled>> Send<T>(this IDispatcher<T> dispatcher, IExampleMessageConsumer.Message message)
        where T : IExampleMessageConsumer
    {
        return IExampleMessageConsumer.Send(dispatcher, message);
    }
}