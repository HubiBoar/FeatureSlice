using OneOf.Types;
using OneOf;

namespace FeatureSlice;

public static partial class Messaging
{
    public delegate Task<OneOf<Success, Disabled, Error>> Consume<TMessage>(TMessage message);
    public delegate Task<OneOf<Success, Disabled, Error>> Dispatch<TMessage>(TMessage message);

    public interface ISetup
    {
        public static ISetup InMemory { get; } = new InMemorySetup();

        public ServiceFactory<Dispatch<TMessage>> Register<TMessage>(
            ConsumerName consumerName,
            ServiceFactory<Consume<TMessage>> consumerFactory)
            where TMessage : notnull;
    }

    internal sealed class InMemorySetup : ISetup
    {
        public ServiceFactory<Dispatch<TMessage>> Register<TMessage>(
            ConsumerName consumerName,
            ServiceFactory<Consume<TMessage>> consumerFactory)
            where TMessage : notnull
        {
            return provider => message => consumerFactory(provider)(message);
        }
    }
}