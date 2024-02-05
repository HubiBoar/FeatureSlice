using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using OneOf;
using OneOf.Types;

namespace FeatureSlice;

public struct Disabled();

public interface IRegistrator<TSelf>
    where TSelf : IRegistrator<TSelf>
{
    public void Register<T>()
        where T : IRegistrableFeature<TSelf>
    {
        T.Register((TSelf)this);
    }
}

public interface IRegistrableFeature<TRegistrator>
    where TRegistrator : IRegistrator<TRegistrator>
{
    public static abstract void Register(TRegistrator registrator);
}

public delegate void Register<TRequest, TResponse, THandler>();

public static partial class Feature
{
    public delegate Task<OneOf<TResponse, Disabled, Error>> Dispatch<TRequest, TResponse, THandler>(TRequest request, IServiceProvider provider)
        where THandler : IHandler<TRequest, TResponse>;

    public interface IHandler<TRequest, TResponse> : IMethod<TRequest, Task<OneOf<TResponse, Error>>>
    {
        public abstract static string Name { get; }

        public interface IRegistrable<THandler> : IHandler<TRequest, TResponse>, IRegistrableFeature<IDispatcherModule>
            where THandler : class, IRegistrable<THandler>
        {
            static void IRegistrableFeature<IDispatcherModule>.Register(IDispatcherModule registrator)
            {
                registrator.Register<TRequest, TResponse, THandler>();
            }

            public static abstract void Register(
                IServiceCollection services,
                IConfiguration configuration,
                ServiceLifetime serviceLifetime,
                Dispatch<TRequest, TResponse, THandler> dispatcher);
        }

        public interface IRegistrable<THandler, TDelegate> : IRegistrable<THandler>
            where THandler : class, IRegistrable<THandler, TDelegate>
            where TDelegate : Delegate
        {
            static void IRegistrable<THandler>.Register(
                IServiceCollection services,
                IConfiguration configuration,
                ServiceLifetime serviceLifetime,
                Dispatch<TRequest, TResponse, THandler> dispatcher)
            {
                services.Add(new ServiceDescriptor(typeof(THandler), typeof(THandler), serviceLifetime));
                services.Add(new ServiceDescriptor(typeof(TDelegate), provider => THandler.Convert(provider, dispatcher), serviceLifetime));
                services.Add(new ServiceDescriptor(typeof(Publisher<TRequest>.Listen), RegisterListener, serviceLifetime));

                Publisher<TRequest>.Listen RegisterListener(IServiceProvider provider)
                {
                    return async request => {
                        var result = await dispatcher(request, provider);
                        return result.Match<OneOf<Success, Error>>(success => new Success(), disabled => new Success(), errror => errror);
                    };
                }
            }

            protected abstract static TDelegate Convert(IServiceProvider provider, Dispatch<TRequest, TResponse, THandler> dispatcher);
        }
    }

    public interface IDispatcher<TRequest, TResponse, THandler>
        where THandler : IHandler<TRequest, TResponse>
    {
        public abstract static Dispatch<TRequest, TResponse, THandler> GetDispatch();
    }
}


public sealed class InMemoryDispatcher<TRequest, TResponse, THandler> : Feature.IDispatcher<TRequest, TResponse, THandler>
    where THandler : Feature.IHandler<TRequest, TResponse>
{
    public static Feature.Dispatch<TRequest, TResponse, THandler> GetDispatch()
    {
        return InMemoryDispatcher.Dispatch<TRequest, TResponse, THandler>;
    }
}

public interface IDispatcherModule : IRegistrator<IDispatcherModule>
{
    public void Register<TRequest, TResponse, THandler>()
        where THandler : class, Feature.IHandler<TRequest, TResponse>.IRegistrable<THandler>;
}

public sealed class InMemoryDispatcher : IDispatcherModule
{
    private readonly IServiceCollection _services;

    private readonly IConfiguration _configuration;

    public InMemoryDispatcher(IServiceCollection services, IConfiguration configuration)
    {
        _services = services;
        _configuration = configuration;
    }

    internal static Task<OneOf<TResponse, Disabled, Error>> Dispatch<TRequest, TResponse, THandler>(
        TRequest request,
        IServiceProvider provider)
        where THandler : Feature.IHandler<TRequest, TResponse>
    {
        return Dispatch(
            request,
            provider.GetRequiredService<THandler>(),
            provider.GetRequiredService<IFeatureManager>(),
            provider.GetServices<Feature.IHandler<TRequest, TResponse>.IPipeline>().ToList());
    }

    internal static async Task<OneOf<TResponse, Disabled, Error>> Dispatch<TRequest, TResponse, THandler>(
        TRequest request,
        THandler handler,
        IFeatureManager featureManager,
        IReadOnlyList<Feature.IHandler<TRequest, TResponse>.IPipeline> pipelines)
        where THandler : Feature.IHandler<TRequest, TResponse>
    {
        var isEnabled = await featureManager.IsEnabledAsync(THandler.Name);
        if(isEnabled == false)
        {
            return new Disabled();
        }

        var pipelinesResult = await pipelines.RunPipeline(request, handler.Handle);
        return pipelinesResult.Match<OneOf<TResponse, Disabled, Error>>(response => response, error => error);
    }

    public void Register<TRequest, TResponse, THandler>()
        where THandler : class, Feature.IHandler<TRequest, TResponse>.IRegistrable<THandler>
    {
        Register<TRequest, TResponse, THandler>(_services, _configuration);
    }

    internal static void Register<TRequest, TResponse, THandler>(IServiceCollection services, IConfiguration configuration)
        where THandler : class, Feature.IHandler<TRequest, TResponse>.IRegistrable<THandler>
    {
        THandler.Register(services, configuration, ServiceLifetime.Singleton, Dispatch<TRequest, TResponse, THandler>);
    }
}


public interface IMessagingModule : IRegistrator<IMessagingModule>
{
    public void Register<TRequest, THandler>()
        where THandler : class, Feature.IHandler<TRequest, Success>.IRegistrable<THandler>;
}

public interface IMessagingDispatcher
{
    public delegate Task<OneOf<Success, Disabled, Error>> Receive<TRequest>(TRequest request);

    public Task<OneOf<Success, Error>> Send<TRequest>(TRequest request, string consumerName);

    public Task<OneOf<Success, Error>> Register<TRequest>(string consumerName, Receive<TRequest> receiver);
}

public sealed class MessagingModule<TDispatcher> : IMessagingModule
    where TDispatcher : IMessagingDispatcher
{
    private readonly IServiceCollection _services;

    private readonly IConfiguration _configuration;

    public MessagingModule(IServiceCollection services, IConfiguration configuration)
    {
        _services = services;
        _configuration = configuration;
    }

    public void Register<TRequest, THandler>()
        where THandler : class, Feature.IHandler<TRequest, Success>.IRegistrable<THandler>
    {
        THandler.Register(_services, _configuration, ServiceLifetime.Singleton, Dispatch<TRequest, THandler>);

        _services.AddHostedService<Registerer>();
        _services.AddSingleton<Registered>(provider => () => Register(
            provider.GetRequiredService<THandler>(),
            provider.GetRequiredService<IFeatureManager>(),
            provider.GetRequiredService<TDispatcher>(),
            provider.GetServices<Feature.IHandler<TRequest, Success>.IPipeline>().ToList()));
    }

    public void Register<TRequest, TResponse, THandler>()
        where THandler : class, Feature.IHandler<TRequest, TResponse>.IRegistrable<THandler>
    {
        InMemoryDispatcher.Register<TRequest, TResponse, THandler>(_services, _configuration);
    }

    internal static Task<OneOf<Success, Error>> Register<TRequest, THandler>(
        THandler handler,
        IFeatureManager featureManager,
        TDispatcher dispatcher,
        IReadOnlyList<Feature.IHandler<TRequest, Success>.IPipeline> pipelines)
        where THandler : Feature.IHandler<TRequest, Success>
    {
        return dispatcher.Register<TRequest>(THandler.Name, request => Receive(request, handler, featureManager, pipelines));
    }

    internal static Task<OneOf<Success, Disabled, Error>> Dispatch<TRequest, THandler>(
        TRequest request,
        IServiceProvider provider)
        where THandler : Feature.IHandler<TRequest, Success>
    {
        return Dispatch<TRequest, THandler>(
            request,
            provider.GetRequiredService<TDispatcher>(),
            provider.GetRequiredService<IFeatureManager>());
    }

    internal static async Task<OneOf<Success, Disabled, Error>> Dispatch<TRequest, THandler>(
        TRequest request,
        TDispatcher dispatcher,
        IFeatureManager featureManager)
        where THandler : Feature.IHandler<TRequest, Success>
    {
        var isEnabled = await featureManager.IsEnabledAsync($"{THandler.Name}-Dispatch");
        if(isEnabled == false)
        {
            return new Disabled();
        }

        var result = await dispatcher.Send(request, THandler.Name);

        return result.Match<OneOf<Success, Disabled, Error>>(success => success, error => error);
    }

    internal static async Task<OneOf<Success, Disabled, Error>> Receive<TRequest, THandler>(
        TRequest request,
        THandler handler,
        IFeatureManager featureManager,
        IReadOnlyList<Feature.IHandler<TRequest, Success>.IPipeline> pipelines)
        where THandler : Feature.IHandler<TRequest, Success>
    {
        var isEnabled = await featureManager.IsEnabledAsync($"{THandler.Name}-Receive");
        if(isEnabled == false)
        {
            return new Disabled();
        }

        var pipelinesResult = await pipelines.RunPipeline(request, handler.Handle);
        return pipelinesResult.Match<OneOf<Success, Disabled, Error>>(success => success, error => error);
    }

    internal delegate Task<OneOf<Success, Error>> Registered();

    internal sealed class Registerer : BackgroundService
    {
        private readonly IReadOnlyCollection<Registered> _registerers;

        public Registerer(IEnumerable<Registered> registerers)
        {
            _registerers = registerers.ToArray();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            foreach(var registerer in _registerers)
            {
                await registerer();
            }
        }
    }
}






