using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using OneOf;
using OneOf.Types;

namespace FeatureSlice.Dispatch;

public interface IMessage
{
    public static abstract string Name { get; }
}

public class MessageContext<T>
    where T : IMessage
{
    public T Message { get; }

    public string MessageId { get; }

    public MessageContext(string messageId, T message)
    {
        MessageId = messageId;
        Message = message;
    }
}

public interface IMessagingConfiguration
{
    public delegate Task<OneOf<Success, Disabled, Retry, Error>> OnMessage<TMessage>(MessageContext<TMessage> context)
        where TMessage : IMessage;

    public Task Register<TMessage>(OnMessage<TMessage> onMessage, string consumerName)
        where TMessage : IMessage;

    public Task Send<TMessage>(TMessage request, string consumerName)
        where TMessage : IMessage;
}

public struct Retry();

public interface IConsumerSetup
{
    public delegate Task Setup();
}

public interface IMessageConsumer<TMessage> : IMethod<MessageContext<TMessage>, Task<OneOf<Success, Retry, Error>>>
    where TMessage : IMessage
{
}

public abstract class MessageConsumer<TSelf, TMessage> : IMessageConsumer<TMessage>, IRegistrable
    where TSelf : MessageConsumer<TSelf, TMessage>, IFeatureName
    where TMessage : IMessage
{
    public abstract Task<OneOf<Success, Retry, Error>> Handle(MessageContext<TMessage> context);

    public delegate Task<OneOf<Success, Disabled>> Dispatcher(TMessage request);

    public static async Task<OneOf<Success, Disabled>> Send(
        TMessage request,
        IMessagingConfiguration configuration,
        IFeatureManager featureManager)
    {
        if(await featureManager.IsEnabledAsync<TSelf>())
        {
            return new Disabled();
        }

        await configuration.Send(request, TSelf.FeatureName);

        return new Success();
    }

    public static Task Setup(
        IMessagingConfiguration configuration,
        TSelf self,
        IFeatureManager featureManager,
        IReadOnlyList<IMessageConsumer<TMessage>.IPipeline> pipelines)
    {
        return configuration.Register<TMessage>(context => Receive(context, self, featureManager, pipelines), TSelf.FeatureName);
    }

    public static async Task<OneOf<Success, Disabled, Retry, Error>> Receive(
        MessageContext<TMessage> context,
        TSelf self,
        IFeatureManager featureManager,
        IReadOnlyList<IMessageConsumer<TMessage>.IPipeline> pipelines)
    {
        if(await featureManager.IsEnabledAsync<TSelf>())
        {
            return new Disabled();
        }

        var result = await pipelines.RunPipeline(context, self.Handle);

        return result.Match<OneOf<Success, Disabled, Retry, Error>>(success => success, retry => retry, error => error);
    }

    public static void Register(IApplicationSetup setup)
    {
        //Add IMessagingConfiguration
        setup.Services.AddFeatureManagement();
        setup.Services.AddSingleton<TSelf>();

        setup.Services.AddSingleton<IConsumerSetup.Setup>(provider => () => Setup(
            provider.GetRequiredService<IMessagingConfiguration>(),
            provider.GetRequiredService<TSelf>(),
            provider.GetRequiredService<IFeatureManager>(),
            provider.GetServices<IMessageConsumer<TMessage>.IPipeline>().ToList())
        );

        setup.Services.AddSingleton<Dispatcher>(provider => request => Send(
            request,
            provider.GetRequiredService<IMessagingConfiguration>(),
            provider.GetRequiredService<IFeatureManager>())
        );
    }
}

public static class MessageConsumerExtensions
{
    public static Task<OneOf<Success, Retry, Error>> HandleInMemory<TMessage>(this IMessageConsumer<TMessage> consumer, TMessage message)
        where TMessage : class, IMessage
    {
        var guid = Guid.NewGuid();
        var id = $"HandleInMemory_{guid}";
        return consumer.Handle(new MessageContext<TMessage>(id, message));
    }
}