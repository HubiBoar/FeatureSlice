using Microsoft.Extensions.DependencyInjection;
using Definit.Dependencies;
using Definit.Results;

namespace FeatureSlice;

public abstract class HandlerBase<TSelf, TRequest, TResponse, TResult, TDependencies>
    where TSelf : HandlerBase<TSelf, TRequest, TResponse, TResult, TDependencies>, new()
    where TDependencies : class, IFromServices<TDependencies>
    where TRequest : notnull
    where TResponse : Result_Base<TResult>
    where TResult : notnull
{
    public delegate Task<TResponse> Handle(TRequest request);
    protected virtual ServiceLifetime ServiceLifetime { get; } = ServiceLifetime.Singleton;

    protected abstract Task<TResponse> OnRequest(TRequest request, TDependencies dependencies);

    internal static void RegisterHandler
    (
        IServiceCollection services,
        ServiceFactory<IHandlerSetup> setupFactory
    )
    {
        var serviceLifetime = new TSelf().ServiceLifetime;
        services.Add(serviceLifetime, GetHandler);
        
        Handle GetHandler(IServiceProvider provider)
        {
            var setup = setupFactory(provider);
            var self = new TSelf();
            var dependencies = TDependencies.Create(provider);

            var handler = setup.GetHandler<TRequest, TResponse>
            (
                provider,
                request => self.OnRequest(request, dependencies)
            );

            return request => handler(request);
        }
    }

    internal static void RegisterHandler
    (
        IServiceCollection services
    )
    {
        var factory = IHandlerSetup.TryRegisterDefault(services);
        RegisterHandler(services, factory);
    }
}

public static class DependencyInjectionExtensions
{
    public static void Add<TService>(
        this IServiceCollection services,
        ServiceLifetime lifetime,
        Func<IServiceProvider, TService> factory)
        where TService : notnull
    {
        services.Add(ServiceDescriptor.Describe(typeof(TService), provider => factory(provider), lifetime));
    }
}

// var manager = provider.GetRequiredService<IFeatureManager>();

// if(await manager.IsEnabledAsync(featureName))
// {
//     return new Disabled($"FeatureFlag [{featureName}] is Disabled");
// }


// public static void AddEndpoint(
//     IHostExtender<WebApplication> extender,
//     Func<IEndpointRouteBuilder, IEndpointConventionBuilder> endpoint)
// {
//     extender.Extend(host => endpoint(host));
// }

// public static void AddEndpoint(
//     IHostExtender<WebApplication> extender,
//     Endpoint endpoint)
// {
//     extender.Map(endpoint);
// }