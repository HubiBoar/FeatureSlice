using Definit.Results;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

public static class FeatureSliceJobExtensions
{
    public static FeatureSliceBase<TRequest, TResult, TResponse>.ISetup WithJob<TRequest, TResult, TResponse>
    (
        this FeatureSliceBase<TRequest, TResult, TResponse>.ISetup options,
        Func<bool> shouldRun,
        Func<TRequest> request
    )
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        options.Extend(services => services.AddHostedService<FeatureSliceJobRunner>());

        options.Extend(services => services.AddSingleton
        (
            provider =>
            {
                var dispatch = options.GetDispatch(provider);
                return new FeatureSliceJob(shouldRun, async _ =>
                {
                    var req = request();
                    if((await dispatch(req)).Is(out Error error))
                    {
                        return error;
                    }
                    else
                    {
                        return Result.Success;
                    }
                });
            }
        ));
        return options;
    }
}