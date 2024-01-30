using FeatureSlice;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using OneOf;
using OneOf.Types;

namespace Samples;

internal static partial class ExampleMessagingConsumer
{
    public sealed record Message() : Messaging.IMessage
    {
        public static string MessageName => "Message";
    }

    public sealed partial class Consumer1 : Messaging.IConsumer<Message>, IRegistrable
    {
        public static string ConsumerName => "Consumer1";

        public Task<OneOf<Success, Messaging.Retry, Error>> Handle(Messaging.Context<Message> request)
        {
            throw new NotImplementedException();
        }
    }

    public sealed partial class Consumer2 : Messaging.IConsumer<Message>, IRegistrable
    {
        public static string ConsumerName => "Consumer2";
        public static string FeatureName => ConsumerName;

        public Task<OneOf<Success, Messaging.Retry, Error>> Handle(Messaging.Context<Message> request)
        {
            throw new NotImplementedException();
        }
    }
}

internal static partial class ExampleMessagingConsumer
{
    public static void Register(IServiceCollection services)
    {
        services.Register<Consumer1>();
        services.Register<Consumer2>();
    }

    public static async Task Run(Consumer1.Dispatch consumer1, Consumer2.Dispatch consumer2, Publisher<Message>.Dispatch dispatch)
    {
        await consumer1(new Message());
        await consumer2(new Message());
        await dispatch(new Message());
    }
}

//Autogeneratoed
internal static partial class ExampleMessagingConsumer
{
    public sealed partial class Consumer1
    {
        public delegate Task Dispatch(Message message);

        public static void Register(IServiceCollection services)
        {
            Messaging.IConsumer<Message>.Setup<Consumer1>.Register<Dispatch>(
                services,
                provider => request => Messaging.IConsumer<Message>.Setup<Consumer1>.DispatchFactory(provider).Invoke(request));
        }
    }

    public sealed partial class Consumer2
    {
        public delegate Task Dispatch(Message message);

        public static void Register(IServiceCollection services)
        {
            Messaging.IConsumer<Message>.Setup<Consumer1>.Register<Dispatch>(
                services,
                provider => request => Messaging.IConsumer<Message>.Setup<Consumer1>.DispatchFactory(provider).Invoke(request));
        }
    }
}