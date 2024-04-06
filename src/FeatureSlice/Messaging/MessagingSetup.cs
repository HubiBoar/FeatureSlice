using Definit.Results;

namespace FeatureSlice;

public static partial class Messaging
{
    public delegate Task<Result.Or<Disabled>> Consume<TMessage>(TMessage message);
    public delegate Task<Result.Or<Disabled>> Dispatch<TMessage>(TMessage message);

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

public static class EnumerableExtensions
{
    public delegate (bool Exists, TTo Value) Is<TFrom, TTo>(TFrom from);

    public static IEnumerable<TTo> SelectWhere<TFrom, TTo>(this IEnumerable<TFrom> enumerable, Is<TFrom, TTo> func)
    {
        foreach(var item in enumerable)
        {
            var result = func(item);
            if(result.Exists)
            {
                yield return result.Value;
            }
        }
    }
}