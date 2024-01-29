using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using OneOf;
using OneOf.Types;

namespace FeatureSlice;

public static partial class Messaging
{
    public partial interface IConsumer<TMessage>
    {
        public interface WithToggle : IMethod<Context<TMessage>, Task<OneOf<Success, Retry, Error>>>, IFeatureName
        {
            public abstract static string ConsumerName { get; }

            public static class Setup<TSelf>
                where TSelf : class, IConsumer<TMessage>.WithToggle
            {
                public static Task Dispatch(TMessage request, TSelf self, IConsumerSetup setup, IFeatureManager featureManager, IReadOnlyList<IPipeline> pipelines)
                {
                    return setup.Send(request, TSelf.ConsumerName, context => Receive(context, self, featureManager, pipelines));
                }

                public static async Task<OneOf<Success, Disabled, Retry, Error>> Receive(Context<TMessage> context, TSelf self, IFeatureManager featureManager, IReadOnlyList<IPipeline> pipelines)
                {
                    if(await featureManager.IsEnabledAsync<TSelf>() == false)
                    {
                        return new Disabled();
                    }

                    return (await pipelines.RunPipeline(context, self.Handle)).Match<OneOf<Success, Disabled, Retry, Error>>(s => s, r => r, e => e);
                }

                public static Task Register(TSelf self, IConsumerSetup setup, IFeatureManager featureManager, IReadOnlyList<IPipeline> pipelines)
                {
                    return setup.Register<TMessage>(TSelf.ConsumerName, context => Receive(context, self, featureManager, pipelines));
                }

                public static Func<TMessage, Task> Factory(IServiceProvider provider)
                {
                    return request => Dispatch(
                        request,
                        provider.GetRequiredService<TSelf>(),
                        provider.GetRequiredService<IConsumerSetup>(),
                        provider.GetRequiredService<IFeatureManager>(),
                        provider.GetServices<IPipeline>().ToList());
                }

                public static void Register<TDispatcher>(IServiceCollection services, Func<IServiceProvider, TDispatcher> factory)
                    where TDispatcher : Delegate
                {
                    services.AddSingleton<TSelf>();
                    services.AddSingleton(factory);
                    services.AddSingleton<Registration>(provider => () => Register(
                        provider.GetRequiredService<TSelf>(),
                        provider.GetRequiredService<IConsumerSetup>(),
                        provider.GetRequiredService<IFeatureManager>(),
                        provider.GetServices<IPipeline>().ToList()));
                }
            }
        }
    }
}