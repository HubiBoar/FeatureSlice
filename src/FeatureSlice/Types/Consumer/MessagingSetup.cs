using OneOf.Types;
using OneOf;

namespace FeatureSlice;

public static partial class Messaging
{
    public interface ISetupProvider
    {
        public static ISetupProvider InMemory { get; } = new InMemorySetup();

        public abstract ISetup GetSetup(IServiceProvider provider);
    }

    public interface ISetup
    {
        public static ISetup InMemory { get; } = new InMemorySetup();

        public delegate Task<OneOf<Success, Disabled, Error>> Receive<TMessage>(TMessage message);

        public Task<OneOf<Success, Disabled, Error>> Send<TMessage>(TMessage message, ConsumerName consumerName, Receive<TMessage> receive);

        public Task<OneOf<Success, Error>> Register<TMessage>(ConsumerName consumerName, Receive<TMessage> receiver);
    }

    internal sealed class InMemorySetup : ISetup, ISetupProvider
    {
        public Task<OneOf<Success, Disabled, Error>> Send<TMessage>(TMessage message, ConsumerName consumerName, ISetup.Receive<TMessage> receive)
        {
            return receive(message);
        }

        public Task<OneOf<Success, Error>> Register<TMessage>(ConsumerName consumerName, ISetup.Receive<TMessage> receiver)
        {
            return Task.FromResult(OneOf<Success, Error>.FromT0(new Success()));
        }

        public ISetup GetSetup(IServiceProvider provider)
        {
            return this;
        }
    }
}