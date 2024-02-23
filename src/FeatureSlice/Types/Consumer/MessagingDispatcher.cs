using OneOf.Types;
using OneOf;
using Microsoft.FeatureManagement;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

public static partial class Messaging
{
    public static class Dispatcher<TMessage>
        where TMessage : notnull
    {
        public delegate Task<OneOf<Success, Error>> Consume(TMessage message);

        public static class Default
        {
            public static async Task<OneOf<Success, Error>> Dispatch(
                TMessage message,
                Dispatch<TMessage> dispatch)
            {
                var result = await dispatch(message);

                return result.Match<OneOf<Success, Error>>(success => success, disabled => new Success(), error => error);
            }

            public static async Task<OneOf<Success, Disabled, Error>> Consume(
                TMessage message,
                Consume consume,
                IReadOnlyList<IPipeline<TMessage, Task<OneOf<Success, Error>>>> pipelines)
            {
                var pipelinesResult = await pipelines.RunPipeline(message, r => consume(r));
                return pipelinesResult.Match<OneOf<Success, Disabled, Error>>(success => success, error => error);
            }

            public static ServiceFactory<DelegateFeatureSlice.Default<TMessage, Success>.Dispatch> Register(
                ConsumerName consumerName,
                ServiceFactory<Consume> getConsumer,
                ISetup setup)
            {
                var dispatchFactory = setup.Register(consumerName, GetConsume);

                return GetDispatch;

                Consume<TMessage> GetConsume(IServiceProvider provider)
                {
                    var consume = getConsumer(provider);
                    var pipelines = provider.GetServices<IPipeline<TMessage, Task<OneOf<Success, Error>>>>().ToList();
                    return message => Consume(message, consume, pipelines);
                }

                DelegateFeatureSlice.Default<TMessage, Success>.Dispatch GetDispatch(IServiceProvider provider)
                {
                    var dispatch = dispatchFactory(provider);

                    return message => Dispatch(message, dispatch);
                }
            }
        }

        public static class WithFlag
        {
            public static async Task<OneOf<Success, Disabled, Error>> Dispatch(
                TMessage message,
                string featureName,
                IFeatureManager featureManager,
                Dispatch<TMessage> dispatch)
            {
                var isEnabled = await featureManager.IsEnabledAsync($"{featureName}-Dispatch");
                if(isEnabled == false)
                {
                    return new Disabled();
                }

                return await dispatch(message);
            }

            public static async Task<OneOf<Success, Disabled, Error>> Consume(
                TMessage message,
                string featureName,
                Consume consume,
                IFeatureManager featureManager,
                IReadOnlyList<IPipeline<TMessage, Task<OneOf<Success, Error>>>> pipelines)
            {
                var isEnabled = await featureManager.IsEnabledAsync($"{featureName}-Consume");
                if(isEnabled == false)
                {
                    return new Disabled();
                }

                var pipelinesResult = await pipelines.RunPipeline(message, r => consume(r));
                return pipelinesResult.Match<OneOf<Success, Disabled, Error>>(success => success, error => error);
            }

            public static ServiceFactory<DelegateFeatureSlice.Flag<TMessage, Success>.Dispatch> Register(
                ConsumerName consumerName,
                string featureName,
                ServiceFactory<Consume> getConsumer,
                ISetup setup)
            {
                var dispatchFactory = setup.Register(consumerName, GetConsume);

                return GetDispatch;

                Consume<TMessage> GetConsume(IServiceProvider provider)
                {
                    var consume = getConsumer(provider);
                    var featureManager = provider.GetRequiredService<IFeatureManager>();
                    var pipelines = provider.GetServices<IPipeline<TMessage, Task<OneOf<Success, Error>>>>().ToList();
                    return message => Consume(message, featureName, consume, featureManager, pipelines);
                }

                DelegateFeatureSlice.Flag<TMessage, Success>.Dispatch GetDispatch(IServiceProvider provider)
                {
                    var dispatch = dispatchFactory(provider);
                    var consume = getConsumer(provider);
                    var featureManager = provider.GetRequiredService<IFeatureManager>();
                    var pipelines = provider.GetServices<IPipeline<TMessage, Task<OneOf<Success, Error>>>>().ToList();

                    return message => Dispatch(message, featureName, featureManager, dispatch);
                }
            }
        }
    }
}