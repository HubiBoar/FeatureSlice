using OneOf.Types;
using OneOf;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

public static partial class Messaging
{
    public struct Retry();

    public delegate Task<OneOf<Success, Disabled, Retry, Error>> Receive<TMessage>(Context<TMessage> context);

    public sealed record Context<TMessage>(TMessage Request);

    public delegate Task Registration();

}