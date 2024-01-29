using OneOf.Types;
using OneOf;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OneOf.Else;

namespace FeatureSlice;

public static partial class Messaging
{
    public interface IMessage
    {
        public static abstract string MessageName { get; }
    }

    public delegate Task Registration();

    public struct Retry();

    public interface ISetup
    {
        public delegate Task<OneOf<Success, Disabled, Retry, Error>> Receive<TMessage>(Messaging<TMessage>.Context context)
            where TMessage : IMessage;

        public Task Send<TMessage>(TMessage message, string consumerName, Receive<TMessage> receive)
            where TMessage : IMessage;

        public Task Send<TMessage>(TMessage message, Receive<TMessage> receive)
            where TMessage : IMessage;

        public Task Register<TMessage>(string consumerName, Receive<TMessage> receiver)
            where TMessage : IMessage;

        public Task Register<TMessage>(Receive<TMessage> receiver)
            where TMessage : IMessage;
    }
}

public sealed partial class Messaging<TMessage> : IRegistrable
    where TMessage : Messaging.IMessage
{
    public interface IReceiver
    {
        Task<OneOf<Success, Disabled, Messaging.Retry, Error>> Receive(Context context);
    }

    public delegate Task Dispatch(TMessage message);
    public sealed record Context(TMessage Request);

    public static void Register(IServiceCollection services)
    {
        services.TryAddSingleton(RegisterDispatch);
        services.TryAddSingleton(RegisterMessagingRegistration);
    
        static Dispatch RegisterDispatch(IServiceProvider provider)
        {
            return message => DispatchMessage(
                message,
                provider.GetRequiredService<Messaging.ISetup>(),
                provider.GetServices<IReceiver>().ToList(),
                provider.GetServices<IMethod<Context, Task<OneOf<Success, Messaging.Retry, Error>>>.IPipeline>().ToList());
        }

        static Messaging.Registration RegisterMessagingRegistration(IServiceProvider provider)
        {
            return () => RegisterInSetup(
                provider.GetRequiredService<Messaging.ISetup>(),
                provider.GetServices<IReceiver>().ToList(),
                provider.GetServices<IMethod<Context, Task<OneOf<Success, Messaging.Retry, Error>>>.IPipeline>().ToList());
        }
    }

    public static Task DispatchMessage(
        TMessage message,
        Messaging.ISetup setup,
        IReadOnlyList<IReceiver> receives,
        IReadOnlyList<IMethod<Context, Task<OneOf<Success, Messaging.Retry, Error>>>.IPipeline> pipelines)
    {
        return setup.Send(message, context => ReceiveMessage(context, receives, pipelines));
    }

    public static async Task<OneOf<Success, Disabled, Messaging.Retry, Error>> ReceiveMessage(
        Context context,
        IReadOnlyList<IReceiver> receives,
        IReadOnlyList<IMethod<Context, Task<OneOf<Success, Messaging.Retry, Error>>>.IPipeline> pipelines)
    {
        return (await pipelines.RunPipeline(context, Handle)).Match<OneOf<Success, Disabled, Messaging.Retry, Error>>(s => s, r => r, e => e);

        async Task<OneOf<Success, Messaging.Retry, Error>> Handle(Context cont)
        {
            foreach(var receiver in receives)
            {
                var result = await receiver.Receive(cont);
                if(result.Is(out Messaging.Retry retry))
                {
                    return retry;   
                }

                if(result.Is(out Error error))
                {
                    return error;
                }
            }

            return new Success();
        }
    }

    public static Task RegisterInSetup(
        Messaging.ISetup setup,
        IReadOnlyList<IReceiver> receives,
        IReadOnlyList<IMethod<Context, Task<OneOf<Success, Messaging.Retry, Error>>>.IPipeline> pipelines)
    {
        return setup.Register<TMessage>(context => ReceiveMessage(context, receives, pipelines));
    }
}