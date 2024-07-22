using FluentServiceBus;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Definit.Results;

namespace FeatureSlice.FluentServiceBus;

public sealed class ServiceBusMessaging : IConsumerDispatcher
{
    private readonly IServiceBusBuilder _builder;
    private readonly ServiceBusClient _client;
    private readonly ServiceBusAdministrationClient _admin;
    private readonly List<Action> _publisherExtensions;

    private ServiceBusMessaging(IServiceBusBuilder builder, IServiceCollection services, ServiceBusClient client, ServiceBusAdministrationClient admin)
    {
        _builder = builder;
        _client = client;
        _admin = admin;
        _publisherExtensions = [];
        services.AddFeatureSlicesExtension<IHost>((host, provider) => provider.GetRequiredService<Task<IRouterPublisher>>());
        services.AddSingleton<Task<IRouterPublisher>>(_ => Build());
    }

    public static void Register(
        IServiceBusBuilder builder,
        IServiceCollection services,
        ServiceBusClient client,
        ServiceBusAdministrationClient admin)
    {
        services.AddSingleton<IConsumerDispatcher>(new ServiceBusMessaging(builder, services, client, admin));
    }

    public static void Create(
        IServiceCollection services,
        ServiceBusClient client,
        ServiceBusAdministrationClient admin)
    {
        services.AddSingleton<IConsumerDispatcher>(new ServiceBusMessaging(new ServiceBusBuilder(), services, client, admin));
    }

    public Dispatch<TRequest, Result, Success> GetDispatcher<TRequest>
    (
        ConsumerName consumerName,
        IServiceProvider provider,
        Dispatch<TRequest, Result, Success> dispatch
    )
        where TRequest : notnull
    {
        var queueName = PathConverter.ToQueueName(consumerName.Name);

        _publisherExtensions.Add(() => {
            _builder
                .AddQueue(queueName)
                .WithConsumer<TRequest>(Consume);
        });

        return Dispatch;

        async Task<Result> Dispatch(TRequest message)
        {
            var publisher = await provider.GetRequiredService<Task<IRouterPublisher>>();
            await publisher.Publish(message, queueName.Value);

            return Result.Success;
        }

        async Task<Result.Or<Abandon>> Consume(TRequest message)
        {
            var result = await dispatch(message);

            return result.Match(
                success => Result.Or<Abandon>.Success,
                error => error);
        }
    }

    private async Task<IRouterPublisher> Build()
    {
        foreach(var extension in _publisherExtensions)
        {
            extension();
        }

        return (await _builder.BuildRouterWithStore(_client, _admin)).Router;
    }

}