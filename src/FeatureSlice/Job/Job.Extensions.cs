using Definit.Results;
using Microsoft.Extensions.DependencyInjection;
using NCrontab;

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

    public static FeatureSliceBase<TRequest, TResult, TResponse>.ISetup WithJob<TRequest, TResult, TResponse>
    (
        this FeatureSliceBase<TRequest, TResult, TResponse>.ISetup options,
        Func<bool> shouldRun,
        TRequest request
    )
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        return options.WithJob(shouldRun, () => request);
    }

    public static FeatureSliceBase<TRequest, TResult, TResponse>.ISetup WithCronJob<TRequest, TResult, TResponse>
    (
        this FeatureSliceBase<TRequest, TResult, TResponse>.ISetup options,
        string cronExpression,
        Func<TRequest> request
    )
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        var cron = CrontabSchedule.TryParse(cronExpression);

        var lastTime = DateTime.UtcNow;

        return options.WithJob(() => 
        {
            var timeNow = DateTime.UtcNow;
            var run = cron.GetNextOccurrences(lastTime, timeNow) is not null;

            lastTime = timeNow;

            return run; 
        }
        , request);
   }

    public static FeatureSliceBase<TRequest, TResult, TResponse>.ISetup WithCronJob<TRequest, TResult, TResponse>
    (
        this FeatureSliceBase<TRequest, TResult, TResponse>.ISetup options,
        string cronExpression,
        TRequest request
    )
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        return options.WithCronJob(cronExpression, () => request);
    }
}