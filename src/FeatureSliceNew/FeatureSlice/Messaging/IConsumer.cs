using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using OneOf;
using OneOf.Types;

namespace FeatureSlice;

public static partial class Messaging
{
    public partial interface IConsumer<TRequest> : IMethod<TRequest, Task<OneOf<Success, Error>>>
    {
        public abstract static string Name { get; }

        public static class Setup<TSelf>
            where TSelf : class, IConsumer<TRequest>
        {
            public static async Task<OneOf<Success, Disabled, Error>> Dispatch(TRequest request, TSelf self, ISetup setup, IFeatureManager featureManager, IReadOnlyList<IPipeline> pipelines)
            {
                var isEnabled = await featureManager.IsEnabledAsync($"{TSelf.Name}-Dispatch");
                if(isEnabled == false)
                {
                    return new Disabled();
                }

                return await setup.Send(request, TSelf.Name, r => Receive(r, self, featureManager, pipelines));
            }

            public static async Task<OneOf<Success, Disabled, Error>> Receive(TRequest request, TSelf self, IFeatureManager featureManager, IReadOnlyList<IPipeline> pipelines)
            {
                var isEnabled = await featureManager.IsEnabledAsync($"{TSelf.Name}-Receive");
                if(isEnabled == false)
                {
                    return new Disabled();
                }

                var pipelinesResult = await pipelines.RunPipeline(request, self.Handle);
                return pipelinesResult.Match<OneOf<Success, Disabled, Error>>(success => success, error => error);
            }

            public static Task RegisterInSetup(TSelf self, ISetup setup, IFeatureManager featureManager, IReadOnlyList<IPipeline> pipelines)
            {
                return setup.Register<TRequest>(TSelf.Name, r => Receive(r, self, featureManager, pipelines));
            }

            public static Func<TRequest, Task<OneOf<Success, Disabled, Error>>> DispatchFactory(IServiceProvider provider)
            {
                return request => Dispatch(
                    request,
                    provider.GetRequiredService<TSelf>(),
                    provider.GetRequiredService<ISetup>(),
                    provider.GetRequiredService<IFeatureManager>(),
                    provider.GetServices<IPipeline>().ToList());
            }

            public static void Register<TDispatcher>(IServiceCollection services, Func<IServiceProvider, TDispatcher> factory)
                where TDispatcher : Delegate
            {
                services.AddSingleton<TSelf>();
                services.AddSingleton(factory);
                services.AddSingleton(RegisterToRegistration);
                services.AddSingleton(RegisterListener);

                static Registration RegisterToRegistration(IServiceProvider provider)
                {
                    return () => RegisterInSetup(
                        provider.GetRequiredService<TSelf>(),
                        provider.GetRequiredService<ISetup>(),
                        provider.GetRequiredService<IFeatureManager>(),
                        provider.GetServices<IPipeline>().ToList());
                }

                static Publisher<TRequest>.Listen RegisterListener(IServiceProvider provider)
                {
                    return async request => {
                        var result = await DispatchFactory(provider)(request);
                        return result.Match<OneOf<Success, Error>>(success => success, disabled => new Success(), errror => errror);
                    };
                }
            }
        }
    }
}

public sealed class ServiceBusDispatcher
{
    public static Task<OneOf<Success, Disabled, Error>> Dispatch<TRequest, THandler>(
        TRequest request,
        IServiceProvider provider)
        where THandler : Feature.IHandler<TRequest, Success>
    {
        return Dispatch(
            request,
            provider.GetRequiredService<THandler>(),
            provider.GetRequiredService<IFeatureManager>(),
            provider.GetServices<Feature.IHandler<TRequest, Success>.IPipeline>().ToList());
    }

    public static async Task<OneOf<Success, Disabled, Error>> Dispatch<TRequest, THandler>(
        TRequest request,
        THandler handler,
        IFeatureManager featureManager,
        IReadOnlyList<Feature.IHandler<TRequest, Success>.IPipeline> pipelines)
        where THandler : Feature.IHandler<TRequest, Success>
    {
        var isEnabled = await featureManager.IsEnabledAsync(THandler.Name);
        if(isEnabled == false)
        {
            return new Disabled();
        }

        var pipelinesResult = await pipelines.RunPipeline(request, handler.Handle);
        return pipelinesResult.Match<OneOf<Success, Disabled, Error>>(success => success, error => error);
    }
}