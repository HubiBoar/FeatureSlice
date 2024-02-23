using OneOf;
using OneOf.Types;
using FluentServiceBus;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using Momolith.Modules;

namespace FeatureSlice.FluentServiceBus;

public sealed class ServiceBusMessaging : Messaging.ISetup
{
    private readonly IServiceBusRouter _router;
    private readonly IHostExtender _extender;

    private ServiceBusMessaging(IServiceBusRouter router, IHostExtender extender)
    {
        _router = router;
        _extender = extender;
    }

    public static ServiceBusMessaging Create(
        IServiceBusRouter router,
        IHostExtender extender,
        IServiceCollection services,
        ServiceBusClient client,
        ServiceBusAdministrationClient admin,
        out Func<Task> builderCallback)
    {
        var messaging = new ServiceBusMessaging(router, extender);

        builderCallback = () => messaging.Build(services, client, admin);

        return messaging;
    }

    public static ServiceBusMessaging Create(
        IHostExtender extender,
        IServiceCollection services,
        ServiceBusClient client,
        ServiceBusAdministrationClient admin,
        out Func<Task> builderCallback)
    {

        return Create(ServiceBusRouter.Create(), extender, services, client, admin, out builderCallback);
    }

    public ServiceFactory<Messaging.Dispatch<TMessage>> Register<TMessage>(
        ConsumerName consumerName,
        ServiceFactory<Messaging.Consume<TMessage>> consumerFactory)
        where TMessage : notnull
    {
        var consumerPath = ConsumerPath.GetConsumerPath(consumerName);
        var queueName = new QueueName(consumerPath.Path);

        _router.AddQueue(queueName)
            .AddExtension((client, admin, queue) => 
            {
                _extender.ExtendAsync(host => StartProcessingQueue(
                    host.Services,
                    client,
                    admin,
                    queue));
                        
                return Task.CompletedTask;
            });

        return provider => message => Dispatch(provider.GetRequiredService<IPublisher>(), message);
        
        async Task StartProcessingQueue(
            IServiceProvider provider,
            ServiceBusClient client,
            ServiceBusAdministrationClient admin,
            Queue queue)
        {
            var consume = consumerFactory(provider);
            var processorOptions = DefaultProcessorOptions();
            var processor = client.CreateProcessor(consumerPath.Path, processorOptions);

            processor.ProcessMessageAsync += args => ProcessMessage(args, consume);
            processor.ProcessErrorAsync += error => ProcessError(error, consumerName);

            await processor.StartProcessingAsync();
        }

        async Task<OneOf<Success, Disabled, Error>> Dispatch(IPublisher publisher, TMessage message)
        {
            await publisher.Publish(message, consumerPath.Path);

            return new Success();
        }
    }

    private Task Build(IServiceCollection services, ServiceBusClient client, ServiceBusAdministrationClient admin)
    {
        return _router.Build(services, client, admin);
    }

    private static ServiceBusProcessorOptions DefaultProcessorOptions()
    {
        return new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = false,
            MaxConcurrentCalls = 1,
        };
    }

    private static Task ProcessMessage<TMessage>(
        ProcessMessageEventArgs args,
        Messaging.Consume<TMessage> consume)
    {
        var serviceBusMessage = args.Message;

        try
        {
            return TryDeserialize<TMessage>(serviceBusMessage).Match(
                async deserializedMessage =>
                {
                    var result = await consume(deserializedMessage);

                    await result.Match(
                        success => args.CompleteMessageAsync(serviceBusMessage),
                        disabled => args.AbandonMessageAsync(serviceBusMessage),
                        error => DeadLetterMessageHelper.Message(args, new Error<string>("Internal Error")));
                },
                deserializationError => DeadLetterMessageHelper.Message(args, deserializationError));
        }
        catch (Exception exception)
        {
            return DeadLetterMessageHelper.Message(args, new Error<string>(exception.Message));
        }
    }
    
    private Task ProcessError(
        ProcessErrorEventArgs error,
        ConsumerName consumerName)
    {
        return Task.CompletedTask;
    }

    private async Task CompleteMessage(
        ProcessMessageEventArgs client,
        ServiceBusReceivedMessage message)
    {
        await client.CompleteMessageAsync(message);
    }

    private static OneOf<TMessage, Error<string>> TryDeserialize<TMessage>(ServiceBusReceivedMessage serviceBusMessage)
    {
        TMessage message;
        var body = serviceBusMessage.Body.ToArray();
        var decodedBody = Encoding.UTF8.GetString(body);
        try
        {
            var newMessage = JsonConvert.DeserializeObject<TMessage>(decodedBody);
            if (newMessage is null)
            {
                return new Error<string>("Message deserialization returned null");
            }
            else
            {
                message = newMessage;
            }
        }
        catch (Exception exception)
        {
            return new Error<string>($"Encountered exception when trying to deserialize message :: {exception.Message}");
        }

        return message;
    }
}

internal class DeadLetterMessageHelper
{
    public static async Task Message(
        ProcessMessageEventArgs client,
        Error<string> error)
    {
        var properties = new Dictionary<string, object>();

        AddDeadLetterProperties(error, properties);

        await client.DeadLetterMessageAsync(client.Message, properties);
    }
    
    private static void AddDeadLetterProperties(Error<string> error, IDictionary<string, object> properties)
    {
        var message = TrimToByteLength(error.Value, 32000);

        properties.Add("DeadLetterErrorMessage", message);
    }
    
    private static string TrimToByteLength(string input, int byteLength)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        var currentBytes = Encoding.UTF8.GetByteCount(input);
        if (currentBytes <= byteLength)
        {
            return input;
        }

        //Are we dealing with all 1-byte chars? Use substring(). This cuts the time in half.
        if (currentBytes == input.Length)
        {
            return input.Substring(0, byteLength);
        }

        var bytesArray = Encoding.UTF8.GetBytes(input);
        Array.Resize(ref bytesArray, byteLength);
        var wordTrimmed = Encoding.UTF8.GetString(bytesArray, 0, byteLength);

        //If a multi-byte sequence was cut apart at the end, the decoder will put a replacement character '�'
        //so trim off the potential trailing '�'
        return wordTrimmed.TrimEnd('�');
    }
}