using Microsoft.Extensions.DependencyInjection;
using Definit.Results;

namespace FeatureSlice;

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

public abstract partial class FeatureSliceBase<TRequest, TResult, TResponse>
    where TRequest : notnull
    where TResult : Result_Base<TResponse>
    where TResponse : notnull
{
    public interface ISetup
    {
        public Func<IServiceProvider, Dispatcher<TRequest, TResult, TResponse>> DispatcherFactory { get; set; }

        public Dispatch<TRequest, TResult, TResponse> GetDispatch(IServiceProvider provider);

        public void Extend(Action<IServiceCollection> services);

        public void Register(IServiceCollection services);
    }
}
