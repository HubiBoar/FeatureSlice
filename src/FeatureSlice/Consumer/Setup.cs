using Definit.Results;

namespace FeatureSlice;

public sealed record ConsumerName(string Name);

public interface IConsumerSetup
{
    public delegate Task<Result> Consume<TMessage>(TMessage message);

    public static IConsumerSetup InMemory { get; } = new InMemorySetup();

    public Consume<TMessage> GetConsumer<TMessage>
    (
        ConsumerName consumerName,
        IServiceProvider provider,
        IHandlerSetup handling,
        Consume<TMessage> consume
    )
        where TMessage : notnull;


    internal sealed class InMemorySetup : IConsumerSetup
    {
        public Consume<TMessage> GetConsumer<TMessage>
        (
            ConsumerName consumerName,
            IServiceProvider provider,
            IHandlerSetup handling,
            Consume<TMessage> consume
        )
            where TMessage : notnull
        {
            return request => handling.GetHandler<TMessage, Result>
            (
                provider,
                req => consume(req)
            )
            (request);
        }
    }
}