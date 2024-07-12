using Definit.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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

    public static ServiceFactory<IConsumerSetup> TryRegisterDefault(IServiceCollection services)
    {
        services.TryAddSingleton<IConsumerSetup, InMemorySetup>();
        return provider => provider.GetRequiredService<IConsumerSetup>();
    }

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