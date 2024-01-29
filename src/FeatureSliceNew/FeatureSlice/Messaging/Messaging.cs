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
        public Task Send<TMessage>(TMessage message, string consumerName, Messaging<TMessage>.Receive receive)
            where TMessage : IMessage;

        public Task Send<TMessage>(TMessage message, Messaging<TMessage>.Receive receive)
            where TMessage : IMessage;

        public Task Register<TMessage>(string consumerName, Messaging<TMessage>.Receive receiver)
            where TMessage : IMessage;

        public Task Register<TMessage>(Messaging<TMessage>.Receive receiver)
            where TMessage : IMessage;
    }
}

public static partial class Messaging<TMessage>
    where TMessage : Messaging.IMessage
{
    public delegate Task<OneOf<Success, Disabled, Messaging.Retry, Error>> Receive(Context context);
    public delegate Task Dispatch(TMessage message);

    public sealed record Context(TMessage Request);

    public static void Register(IServiceCollection services)
    {
        services.TryAddSingleton<Dispatch>(provider => message =>
            DispatchMessage(
                message,
                provider.GetRequiredService<Messaging.ISetup>(),
                provider.GetServices<Receive>().ToList(),
                provider.GetServices<IMethod<Context, Task<OneOf<Success, Messaging.Retry, Error>>>.IPipeline>().ToList()));

        services.TryAddSingleton<Messaging.Registration>(provider => () =>
            RegisterInSetup(
                provider.GetRequiredService<Messaging.ISetup>(),
                provider.GetServices<Receive>().ToList(),
                provider.GetServices<IMethod<Context, Task<OneOf<Success, Messaging.Retry, Error>>>.IPipeline>().ToList()));
    }

    public static Task DispatchMessage(
        TMessage message,
        Messaging.ISetup setup,
        IReadOnlyList<Receive> receives,
        IReadOnlyList<IMethod<Context, Task<OneOf<Success, Messaging.Retry, Error>>>.IPipeline> pipelines)
    {
        return setup.Send(message, context => ReceiveMessage(context, receives, pipelines));
    }

    public static async Task<OneOf<Success, Disabled, Messaging.Retry, Error>> ReceiveMessage(
        Context context,
        IReadOnlyList<Receive> receives,
        IReadOnlyList<IMethod<Context, Task<OneOf<Success, Messaging.Retry, Error>>>.IPipeline> pipelines)
    {
        return (await pipelines.RunPipeline(context, Handle)).Match<OneOf<Success, Disabled, Messaging.Retry, Error>>(s => s, r => r, e => e);

        async Task<OneOf<Success, Messaging.Retry, Error>> Handle(Context cont)
        {
            foreach(var receiver in receives)
            {
                var result = await receiver(cont);
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
        IReadOnlyList<Receive> receives,
        IReadOnlyList<IMethod<Context, Task<OneOf<Success, Messaging.Retry, Error>>>.IPipeline> pipelines)
    {
        return setup.Register<TMessage>(context => ReceiveMessage(context, receives, pipelines));
    }
}