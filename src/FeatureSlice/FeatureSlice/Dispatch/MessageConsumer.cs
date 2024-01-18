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
    public static abstract string MessageName { get; }
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
    public Task Setup(IMessagingConfiguration configuration);
}

public interface IMessageConsumer<TMessage> : IMethod<MessageContext<TMessage>, Task<OneOf<Success, Retry, Error>>>
    where TMessage : IMessage
{
}

public abstract class MessageConsumer<TSelf, TMessage> : IMessageConsumer<TMessage>, IRegistrable<IMessagingConfiguration>
    where TSelf : MessageConsumer<TSelf, TMessage>, IFeatureName
    where TMessage : IMessage
{
    public abstract Task<OneOf<Success, Retry, Error>> Handle(MessageContext<TMessage> context);

    public interface IDispatcher
    {
        public Task Send(TMessage request);
    }

    private sealed class Dispatcher : IDispatcher, IConsumerSetup
    {
        private readonly TSelf _self;
        private readonly IFeatureManager _featureManager;
        private readonly IReadOnlyList<IMessageConsumer<TMessage>.IPipeline> _pipelines;
        private readonly IMessagingConfiguration _configuration;

        public Dispatcher(TSelf self, IFeatureManager featureManager, IEnumerable<IMessageConsumer<TMessage>.IPipeline> pipelines, IMessagingConfiguration configuration)
        {
            _self = self;
            _featureManager = featureManager;
            _pipelines = pipelines.ToList();
            _configuration = configuration;
        }

        public Task Send(TMessage request)
        {
            return _configuration.Send(request, TSelf.FeatureName);
        }

        public Task Setup(IMessagingConfiguration configuration)
        {
            return configuration.Register<TMessage>(Receive, TSelf.FeatureName);
        }

        private async Task<OneOf<Success, Disabled, Retry, Error>> Receive(MessageContext<TMessage> context)
        {
            if(await _featureManager.IsEnabledAsync<TSelf>())
            {
                return new Disabled();
            }

            var result = await _pipelines.RunPipeline(context, _self.Handle);

            return result.Match<OneOf<Success, Disabled, Retry, Error>>(success => success, retry => retry, error => error);
        }
    }

    public static void Register<T>(IServiceCollection services)
        where T : class, IMessagingConfiguration
    {
        services.AddFeatureManagement();
        services.AddSingleton<TSelf>();
        services.AddSingleton<T>();
        services.AddSingleton<IConsumerSetup, Dispatcher>();
        services.AddSingleton<IDispatcher, Dispatcher>();
    }
}