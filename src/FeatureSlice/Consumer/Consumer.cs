using Definit.Results;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

public interface IConsumerSetup<TRequest> : IHandlerSetup<TRequest, Result, Success>
    where TRequest : notnull
{
}

public static class FeatureSliceConsumerExtensions
{
    public static FeatureSliceBase<TSelf, TRequest, Result, Success>.Options AsConsumer<TSelf, TRequest>
    (
        this FeatureSliceBase<TSelf, TRequest, Result, Success>.Options options
    )
        where TSelf : FeatureSliceBase<TSelf, TRequest, Result, Success>, new()
        where TRequest : notnull
    {
        options.HandlerFactory = provider => provider.GetRequiredService<IConsumerSetup<TRequest>>();
        return options;
    }
}