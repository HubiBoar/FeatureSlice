using Microsoft.Extensions.DependencyInjection;
using Definit.Dependencies;
using Definit.Results;

namespace FeatureSlice;

public abstract class HandlerBase<TSelf, TRequest, TResponse, TDependencies>
    where TSelf : HandlerBase<TSelf, TRequest, TResponse, TDependencies>, new()
    where TDependencies : class, IFromServices<TDependencies>
    where TRequest : notnull
    where TResponse : notnull
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
}

public abstract class HandlerBase<TSelf, TRequest, TDependencies> 
    : HandlerBase<TSelf, TRequest, Result, TDependencies>
    where TSelf : HandlerBase<TSelf, TRequest, TDependencies>, new()
    where TDependencies : class, IFromServices<TDependencies>
    where TRequest : notnull
{
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