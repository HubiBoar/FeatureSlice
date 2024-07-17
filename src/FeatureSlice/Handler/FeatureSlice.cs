using Microsoft.Extensions.DependencyInjection;
using Definit.Results;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FeatureSlice;

public delegate Task<TResult> Dispatch<TRequest, TResult, TResponse>(TRequest request)
    where TRequest : notnull
    where TResult : Result_Base<TResponse>
    where TResponse : notnull;

public sealed record FeatureSliceOptions(IServiceCollection Services);

public static class DispatcherExtensions
{
    public static FeatureSliceOptions AddFeatureSlices(this IServiceCollection services)
    {
        services.AddSwaggerGen(options => options.SetCustomSchemaId());
        return new FeatureSliceOptions(services);
    }

    public static FeatureSliceOptions DefaultDispatcher(this FeatureSliceOptions options)
    {
        options.Services.AddSingleton(typeof(IDispatcher<,,>), typeof(IDispatcher<,,>.Default));

        return options;
    }
}

public interface IDispatcher<TRequest, TResult, TResponse>
    where TRequest : notnull
    where TResult : Result_Base<TResponse>
    where TResponse : notnull
{
    public Dispatch<TRequest, TResult, TResponse> GetDispatcher
    (
        IServiceProvider provider,
        Dispatch<TRequest, TResult, TResponse> dispatch
    );

    public sealed class Default : IDispatcher<TRequest, TResult, TResponse>
    {
        public Dispatch<TRequest, TResult, TResponse> GetDispatcher
        (
            IServiceProvider provider,
            Dispatch<TRequest, TResult, TResponse> dispatch
        )
        {
            Console.WriteLine("Handler");
            return dispatch;
        }
    }
}

public abstract class FeatureSlice<TSelf, TRequest, TResponse> : FeatureSliceBase<TSelf, TRequest, Result<TResponse>, TResponse>
    where TSelf : FeatureSlice<TSelf, TRequest, TResponse>, new()
    where TRequest : notnull
    where TResponse : notnull
{
}

public abstract class FeatureSlice<TSelf, TRequest> : FeatureSliceBase<TSelf, TRequest, Result, Success>
    where TSelf : FeatureSlice<TSelf, TRequest>, new()
    where TRequest : notnull
{
}

public abstract partial class FeatureSliceBase<TRequest, TResult, TResponse>
    where TRequest : notnull
    where TResult : Result_Base<TResponse>
    where TResponse : notnull
{
    public interface ISetup
    {
        public Func<IServiceProvider, IDispatcher<TRequest, TResult, TResponse>> DispatcherFactory { get; set; }

        public Func<IServiceProvider, Dispatch<TRequest, TResult, TResponse>> DispatchFactory { get; }

        public void Register(IServiceCollection services);
    }
}

public abstract partial class FeatureSliceBase<TSelf, TRequest, TResult, TResponse>
    : FeatureSliceBase<TRequest, TResult, TResponse>
    where TSelf : FeatureSliceBase<TSelf, TRequest, TResult, TResponse>, new()
    where TRequest : notnull
    where TResult : Result_Base<TResponse>
    where TResponse : notnull
{
    public delegate Task<TResult> Dispatch(TRequest request);

    public abstract ISetup Setup { get; }

    public static void Register
    (
        IServiceCollection services
    )
    {
        var self = new TSelf();

        self.Setup.Register(services);
    }
}