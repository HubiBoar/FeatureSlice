using OneOf.Types;
using OneOf;
using Microsoft.FeatureManagement;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

public static partial class Messaging
{
    public delegate Task Registration();
    public delegate Task<OneOf<Success, Disabled, Error>> Receive<TMessage>(TMessage message);

    public interface ISetup
    {
        public Task<OneOf<Success, Disabled, Error>> Send<TMessage>(TMessage message, ConsumerName consumerName, Receive<TMessage> receive);

        public Task<OneOf<Success, Error>> Register<TMessage>(ConsumerName consumerName, Receive<TMessage> receiver);
    }

    public static class Dispatcher<TMessage>
    {
        public delegate Task<OneOf<Success, Error>> Consume(TMessage message);

        public static class Default
        {
            public static async Task<OneOf<Success, Error>> Dispatch(
                TMessage message,
                ConsumerName consumerName,
                Consume consume,
                ISetup setup,
                IReadOnlyList<IPipeline<TMessage, Task<OneOf<Success, Error>>>> pipelines)
            {
                var result = await setup.Send(message, consumerName, r => Receive(r, consume, pipelines));

                return result.Match<OneOf<Success, Error>>(success => success, disabled => new Success(), error => error);
            }

            public static async Task<OneOf<Success, Disabled, Error>> Receive(
                TMessage message,
                Consume consume,
                IReadOnlyList<IPipeline<TMessage, Task<OneOf<Success, Error>>>> pipelines)
            {
                var pipelinesResult = await pipelines.RunPipeline(message, r => consume(r));
                return pipelinesResult.Match<OneOf<Success, Disabled, Error>>(success => success, error => error);
            }

            public static Func<IServiceProvider, DelegateFeatureSlice.Default<TMessage, Success>.Dispatch> Register(
                IServiceCollection services,
                ConsumerName consumerName,
                Func<IServiceProvider, Consume> getConsumer,
                Func<IServiceProvider, ISetup> getSetup)
            {
                services.AddSingleton<Registration>(provider => () => GetRegistration(provider));

                return GetDispatch;

                Task GetRegistration(IServiceProvider provider)
                {
                    var setup = getSetup(provider);
                    var consume = getConsumer(provider);
                    var pipelines = provider.GetServices<IPipeline<TMessage, Task<OneOf<Success, Error>>>>().ToList();
                    return setup.Register<TMessage>(consumerName, r => Receive(r, consume, pipelines));
                }

                DelegateFeatureSlice.Default<TMessage, Success>.Dispatch GetDispatch(IServiceProvider provider)
                {
                    var setup = getSetup(provider);
                    var consume = getConsumer(provider);
                    var pipelines = provider.GetServices<IPipeline<TMessage, Task<OneOf<Success, Error>>>>().ToList();

                    return message => Dispatch(message, consumerName, consume, setup, pipelines);
                }
            }
        }

        public static class WithFlag
        {
            public static async Task<OneOf<Success, Disabled, Error>> Dispatch(
                TMessage message,
                ConsumerName consumerName,
                string featureName,
                Consume consume,
                ISetup setup,
                IFeatureManager featureManager,
                IReadOnlyList<IPipeline<TMessage, Task<OneOf<Success, Error>>>> pipelines)
            {
                var isEnabled = await featureManager.IsEnabledAsync($"{featureName}-Dispatch");
                if(isEnabled == false)
                {
                    return new Disabled();
                }

                return await setup.Send(message, consumerName, r => Receive(r, featureName, consume, featureManager, pipelines));
            }

            public static async Task<OneOf<Success, Disabled, Error>> Receive(
                TMessage message,
                string featureName,
                Consume consume,
                IFeatureManager featureManager,
                IReadOnlyList<IPipeline<TMessage, Task<OneOf<Success, Error>>>> pipelines)
            {
                var isEnabled = await featureManager.IsEnabledAsync($"{featureName}-Receive");
                if(isEnabled == false)
                {
                    return new Disabled();
                }

                var pipelinesResult = await pipelines.RunPipeline(message, r => consume(r));
                return pipelinesResult.Match<OneOf<Success, Disabled, Error>>(success => success, error => error);
            }

            public static Func<IServiceProvider, DelegateFeatureSlice.Flag<TMessage, Success>.Dispatch> Register(
                IServiceCollection services,
                ConsumerName consumerName,
                string featureName,
                Func<IServiceProvider, Consume> getConsumer,
                Func<IServiceProvider, ISetup> getSetup)
            {
                services.AddSingleton<Registration>(provider => () => GetRegistration(provider));

                return GetDispatch;

                Task GetRegistration(IServiceProvider provider)
                {
                    var setup = getSetup(provider);
                    var consume = getConsumer(provider);
                    var featureManager = provider.GetRequiredService<IFeatureManager>();
                    var pipelines = provider.GetServices<IPipeline<TMessage, Task<OneOf<Success, Error>>>>().ToList();
                    return setup.Register<TMessage>(consumerName, r => Receive(r, featureName, consume, featureManager, pipelines));
                }

                DelegateFeatureSlice.Flag<TMessage, Success>.Dispatch GetDispatch(IServiceProvider provider)
                {
                    var setup = getSetup(provider);
                    var consume = getConsumer(provider);
                    var featureManager = provider.GetRequiredService<IFeatureManager>();
                    var pipelines = provider.GetServices<IPipeline<TMessage, Task<OneOf<Success, Error>>>>().ToList();

                    return message => Dispatch(message, consumerName, featureName, consume, setup, featureManager, pipelines);
                }
            }
        }
    }
}