// using FluentServiceBus;
// using Azure.Messaging.ServiceBus;
// using Azure.Messaging.ServiceBus.Administration;
// using Momolith.Modules;
// using Microsoft.Extensions.Hosting;
// using Microsoft.Extensions.DependencyInjection;
// using Definit.Results;

// namespace FeatureSlice.FluentServiceBus;

// public sealed class ServiceBusMessaging : IConsumerSetup
// {
//     private IRouterPublisher Publisher { get; set; } = null!;
//     private readonly IServiceBusBuilder _builder;
//     private readonly ServiceBusClient _client;
//     private readonly ServiceBusAdministrationClient _admin;
//     private readonly List<Func<IHost, Task>> _hostExtensions;

//     private ServiceBusMessaging(IServiceBusBuilder builder, IServiceCollection services, IHostExtender extender, ServiceBusClient client, ServiceBusAdministrationClient admin)
//     {
//         _builder = builder;
//         _client = client;
//         _admin = admin;
//         _hostExtensions = [];
//         extender.ExtendAsync(Build);
//         services.AddSingleton<IRouterPublisher>(_ => Publisher);
//     }

//     public static IConsumerSetup Create(
//         IServiceBusBuilder builder,
//         IServiceCollection services,
//         IHostExtender extender,
//         ServiceBusClient client,
//         ServiceBusAdministrationClient admin)
//     {
//         return new ServiceBusMessaging(builder, services, extender, client, admin);
//     }

//     public static IConsumerSetup Create(
//         IServiceCollection services,
//         IHostExtender extender,
//         ServiceBusClient client,
//         ServiceBusAdministrationClient admin)
//     {
//         return new ServiceBusMessaging(new ServiceBusBuilder(), services, extender, client, admin);
//     }

//     public ServiceFactory<IConsumerSetup.Consume<TMessage>> GetConsumer<TMessage>(
//         ConsumerName consumerName,
//         ServiceFactory<IConsumerSetup.Consume<TMessage>> consumerFactory)
//         where TMessage : notnull
//     {
//         var queueName = PathConverter.ToQueueName(consumerName.Name);

//         _hostExtensions.Add(host => {
//             var consumer = consumerFactory(host.Services);

//             _builder
//                 .AddQueue(queueName)
//                 .WithConsumer<TMessage>(message => Consume(message, consumer));

//             return Task.CompletedTask;
//         });

//         return provider => message => Dispatch(provider, message, queueName);

//         static async Task<Result> Dispatch(IServiceProvider provider, TMessage message, QueueName queueName)
//         {
//             var publisher = provider.GetRequiredService<IRouterPublisher>();
//             await publisher.Publish(message, queueName.Value);

//             return Result.Success;
//         }

//         static async Task<Result.Or<Abandon>> Consume(TMessage message, IConsumerSetup.Consume<TMessage> consume)
//         {
//             var result = await consume(message);

//             return result.Match<Result.Or<Abandon>>(
//                 success => success,
//                 abandon => new Abandon(),
//                 error => error);
//         }
//     }

//     private async Task Build(IHost host)
//     {
//         foreach(var extension in _hostExtensions)
//         {
//             await extension(host);
//         }

//         var built = await _builder.BuildRouterWithStore(_client, _admin);
//         Publisher = built.Router;
//     }
// }