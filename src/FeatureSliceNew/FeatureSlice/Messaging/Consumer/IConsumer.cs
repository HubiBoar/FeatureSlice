using OneOf.Types;
using OneOf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FeatureSlice;

public static partial class Messaging<TMessage>
{
    public partial interface IConsumer : IMethod<Context, Task<OneOf<Success, Messaging.Retry, Error>>>
    {
        public abstract static string ConsumerName { get; }

        public static class Setup<TSelf>
            where TSelf : class, IConsumer
        {
            public static Task Dispatch(TMessage request, TSelf self, Messaging.ISetup setup, IReadOnlyList<IPipeline> pipelines)
            {
                return setup.Send(request, TSelf.ConsumerName, context => Receive(context, self, pipelines));
            }

            public static async Task<OneOf<Success, Disabled,  Messaging.Retry, Error>> Receive(Context context, TSelf self, IReadOnlyList<IPipeline> pipelines)
            {
                return (await pipelines.RunPipeline(context, self.Handle)).Match<OneOf<Success, Disabled,  Messaging.Retry, Error>>(s => s, r => r, e => e);
            }

            public static Task RegisterInSetup(TSelf self,  Messaging.ISetup setup, IReadOnlyList<IPipeline> pipelines)
            {
                return setup.Register<TMessage>(TSelf.ConsumerName, context => Receive(context, self, pipelines));
            }

            public static Func<TMessage, Task> Factory(IServiceProvider provider)
            {
                return request => Dispatch(
                    request,
                    provider.GetRequiredService<TSelf>(),
                    provider.GetRequiredService<Messaging.ISetup>(),
                    provider.GetServices<IPipeline>().ToList());
            }

            public static void Register<TDispatcher>(IServiceCollection services, Func<IServiceProvider, TDispatcher> dispatcherFactory)
                where TDispatcher : Delegate
            {
                //Subscriber
                services.AddSingleton(RegisterReceive);
                Messaging<TMessage>.Register(services);

                //Consumer
                services.AddSingleton<TSelf>();
                services.AddSingleton(dispatcherFactory);
                services.AddSingleton<Messaging.Registration>(provider => () =>
                    RegisterInSetup(
                        provider.GetRequiredService<TSelf>(),
                        provider.GetRequiredService<Messaging.ISetup>(),
                        provider.GetServices<IPipeline>().ToList()));
        

                static Messaging<TMessage>.Receive RegisterReceive(IServiceProvider provider)
                {
                    return async context => (await provider.GetRequiredService<TSelf>().Handle(context)).Match<OneOf<Success, Disabled, Messaging.Retry, Error>>(s => s, r => r, e => e);
                }
            }
        }
    }
}