using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using OneOf;
using OneOf.Types;

namespace FeatureSlice;

public sealed partial class Messaging<TMessage>
{
    public partial interface IConsumer : IMethod<Context, Task<OneOf<Success, Messaging.Retry, Error>>>
    {
        public abstract static string ConsumerName { get; }

        public class Setup<TSelf>
            where TSelf : class, IConsumer
        {
            public static Task Dispatch(TMessage request, TSelf self, Messaging.ISetup setup, IFeatureManager featureManager, IReadOnlyList<IPipeline> pipelines)
            {
                return setup.Send(request, TSelf.ConsumerName, context => Receive(context, self, featureManager, pipelines));
            }

            public static async Task<OneOf<Success, Disabled, Messaging.Retry, Error>> Receive(Context context, TSelf self, IFeatureManager featureManager, IReadOnlyList<IPipeline> pipelines)
            {
                if(await featureManager.IsEnabledAsync(TSelf.ConsumerName) == false)
                {
                    return new Disabled();
                }

                return (await pipelines.RunPipeline(context, self.Handle)).Match<OneOf<Success, Disabled, Messaging.Retry, Error>>(s => s, r => r, e => e);
            }

            public static Task RegisterInSetup(TSelf self, Messaging.ISetup setup, IFeatureManager featureManager, IReadOnlyList<IPipeline> pipelines)
            {
                return setup.Register<TMessage>(TSelf.ConsumerName, context => Receive(context, self, featureManager, pipelines));
            }

            public static Func<TMessage, Task> Factory(IServiceProvider provider)
            {
                return request => Dispatch(
                    request,
                    provider.GetRequiredService<TSelf>(),
                    provider.GetRequiredService<Messaging.ISetup>(),
                    provider.GetRequiredService<IFeatureManager>(),
                    provider.GetServices<IPipeline>().ToList());
            }

            public static void Register<TDispatcher>(IServiceCollection services, Func<IServiceProvider, TDispatcher> factory)
                where TDispatcher : Delegate
            {
                //Subscriber
                services.AddSingleton(RegisterReceive);
                services.Register<Messaging<TMessage>>();

                //Consumer
                services.AddSingleton<TSelf>();
                services.AddSingleton(factory);
                services.AddSingleton(RegisterToRegistration);

                static Messaging.Registration RegisterToRegistration(IServiceProvider provider)
                {
                    return () => RegisterInSetup(
                        provider.GetRequiredService<TSelf>(),
                        provider.GetRequiredService<Messaging.ISetup>(),
                        provider.GetRequiredService<IFeatureManager>(),
                        provider.GetServices<IPipeline>().ToList());
                }

                static Messaging<TMessage>.Receive RegisterReceive(IServiceProvider provider)
                {
                    return context => HandleReceive(context, provider.GetRequiredService<TSelf>(), provider.GetRequiredService<IFeatureManager>());
                }
            }

            public static async Task<OneOf<Success, Disabled, Messaging.Retry, Error>> HandleReceive(Context context, TSelf self, IFeatureManager featureManager)
            {
                if(await featureManager.IsEnabledAsync(TSelf.ConsumerName) == false)
                {
                    return new Disabled();
                }

                return (await self.Handle(context)).Match<OneOf<Success, Disabled, Messaging.Retry, Error>>(s => s, r => r, e => e);
            }
        }
    }
}