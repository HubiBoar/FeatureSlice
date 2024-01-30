using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using OneOf;
using OneOf.Types;

namespace FeatureSlice;

public static partial class Messaging
{
    public partial interface IConsumer<TMessage> : IMethod<Context<TMessage>, Task<OneOf<Success, Retry, Error>>>
        where TMessage : IMessage
    {
        public abstract static string Name { get; }

        public static class Setup<TSelf>
            where TSelf : class, IConsumer<TMessage>
        {
            public static Task<OneOf<Success, Error>> Dispatch(TMessage request, TSelf self, ISetup setup, IFeatureManager featureManager, IReadOnlyList<IPipeline> pipelines)
            {
                return setup.Send(request, TSelf.Name, context => Receive(context, self, featureManager, pipelines));
            }

            public static async Task<OneOf<Success, Disabled, Retry, Error>> Receive(Context<TMessage> context, TSelf self, IFeatureManager featureManager, IReadOnlyList<IPipeline> pipelines)
            {
                if(await featureManager.IsEnabledAsync(TSelf.Name) == false)
                {
                    return new Disabled();
                }

                return (await pipelines.RunPipeline(context, self.Handle)).Match<OneOf<Success, Disabled, Retry, Error>>(s => s, r => r, e => e);
            }

            public static Task RegisterInSetup(TSelf self, ISetup setup, IFeatureManager featureManager, IReadOnlyList<IPipeline> pipelines)
            {
                return setup.Register<TMessage>(TSelf.Name, context => Receive(context, self, featureManager, pipelines));
            }

            public static Func<TMessage, Task<OneOf<Success, Error>>> DispatchFactory(IServiceProvider provider)
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

                static Publisher<TMessage>.Listen RegisterListener(IServiceProvider provider)
                {
                    return (request) => DispatchFactory(provider)(request);
                }
            }
        }
    }
}