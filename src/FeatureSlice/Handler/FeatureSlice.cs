using Microsoft.Extensions.DependencyInjection;
using Definit.Results;

namespace FeatureSlice;

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

public abstract partial class FeatureSliceBase<TSelf, TRequest, TResult, TResponse>
    where TSelf : FeatureSliceBase<TSelf, TRequest, TResult, TResponse>, new()
    where TRequest : notnull
    where TResult : Result_Base<TResponse>
    where TResponse : notnull
{

    public delegate Task<TResult> Dispatch(TRequest request);

    public abstract Options Setup { get; }

    public static Options Handle<TDep0>(Func<TRequest, TDep0, Task<TResult>> handle)
        where TDep0 : notnull
    {
        return new Options
        (
            provider =>
                request =>
                    handle
                    (
                        request,
                        provider.GetRequiredService<TDep0>()
                    )
        );
    }

    public static Options Handle<TDep0, TDep1>(Func<TRequest, TDep0, TDep1, Task<TResult>> handle)
        where TDep0 : notnull
        where TDep1 : notnull
    {
        return new Options
        (
            provider =>
                request =>
                    handle
                    (
                        request,
                        provider.GetRequiredService<TDep0>(), 
                        provider.GetRequiredService<TDep1>()
                    )
        );
    }

    public static void Register
    (
        IServiceCollection services
    )
    {
        var self = new TSelf();

        self.Setup.Register
        (
            services,
            (provider, dispatch) => dispatch,
            ServiceLifetime.Singleton
        );
    }
}
