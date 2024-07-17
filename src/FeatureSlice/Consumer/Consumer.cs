using Definit.Results;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

public interface IConsumerDispatcher<TRequest> : IDispatcher<TRequest, Result, Success>
    where TRequest : notnull
{
    public sealed class ConsumerDefault : IConsumerDispatcher<TRequest>
    {
        public Dispatch<TRequest, Result, Success> GetDispatcher
        (
            IServiceProvider provider,
            Dispatch<TRequest, Result, Success> dispatch
        )
        {
            return dispatch;
        }
    }
}

public static class FeatureSliceConsumerExtensions
{
    public static FeatureSliceBase<TRequest, Result, Success>.ISetup AsConsumer<TRequest>
    (
        this FeatureSliceBase<TRequest, Result, Success>.ISetup options
    )
        where TRequest : notnull
    {
        options.DispatcherFactory = provider => provider.GetRequiredService<IConsumerDispatcher<TRequest>>();
        return options;
    }

    public static FeatureSliceOptions DefaultConsumerDispatcher(this FeatureSliceOptions options)
    {
        options.Services.AddSingleton(typeof(IConsumerDispatcher<>), typeof(IConsumerDispatcher<>.ConsumerDefault));

        return options;
    }
}