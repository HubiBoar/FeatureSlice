using OneOf.Types;
using OneOf;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

public static partial class Messaging
{
    public interface IConsumerSetup
    {
        public Task Send<TMessage>(TMessage message, string consumerName, Receive<TMessage> receive);

        public Task Register<TMessage>(string consumerName, Receive<TMessage> receiver);
    }

    public partial interface IConsumer<TMessage> : IMethod<Context<TMessage>, Task<OneOf<Success, Retry, Error>>>
    {
        public abstract static string ConsumerName { get; }

        public static class Setup<TSelf>
            where TSelf : class, IConsumer<TMessage>
        {
            public static Task Dispatch(TMessage request, TSelf self, IConsumerSetup setup, IReadOnlyList<IPipeline> pipelines)
            {
                return setup.Send(request, TSelf.ConsumerName, context => Receive(context, self, pipelines));
            }

            public static async Task<OneOf<Success, Disabled, Retry, Error>> Receive(Context<TMessage> context, TSelf self, IReadOnlyList<IPipeline> pipelines)
            {
                return (await pipelines.RunPipeline(context, self.Handle)).Match<OneOf<Success, Disabled, Retry, Error>>(s => s, r => r, e => e);
            }

            public static Task Register(TSelf self, IConsumerSetup setup, IReadOnlyList<IPipeline> pipelines)
            {
                return setup.Register<TMessage>(TSelf.ConsumerName, context => Receive(context, self, pipelines));
            }

            public static Func<TMessage, Task> Factory(IServiceProvider provider)
            {
                return request => Dispatch(
                    request,
                    provider.GetRequiredService<TSelf>(),
                    provider.GetRequiredService<IConsumerSetup>(),
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
                    provider.GetServices<IPipeline>().ToList()));
            }
        }
    }
        
}

public sealed class ServiceBusConsumerSetup : Messaging.IConsumerSetup
{
    public Task Register<TMessage>(string queueName, Messaging.Receive<TMessage> receiver)
    {
        throw new NotImplementedException();
    }

    public Task Send<TMessage>(TMessage message, string queueName, Messaging.Receive<TMessage> receive)
    {
        throw new NotImplementedException();
    }
}