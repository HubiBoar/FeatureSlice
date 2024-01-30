using OneOf.Types;
using OneOf;

namespace FeatureSlice;

public static partial class Messaging
{
    public struct Retry();

    public interface IMessage
    {
        public static abstract string MessageName { get; }
    }

    public delegate Task Registration();

    public interface ISetup
    {
        public delegate Task<OneOf<Success, Disabled, Retry, Error>> Receive<TMessage>(Context<TMessage> context)
            where TMessage : IMessage;

        public Task<OneOf<Success, Error>> Send<TMessage>(TMessage message, string consumerName, Receive<TMessage> receive)
            where TMessage : IMessage;

        public Task<OneOf<Success, Error>> Register<TMessage>(string consumerName, Receive<TMessage> receiver)
            where TMessage : IMessage;
    }

    public sealed record Context<TMessage>(TMessage Request)
        where TMessage : IMessage;
}