using Definit.Results;
using Microsoft.Extensions.DependencyInjection;
using NCrontab;

namespace FeatureSlice;

public static class FeatureSliceJobExtensions
{
    public static IFeatureSliceSetup<TRequest, TResult, TResponse> MapJob<TRequest, TResult, TResponse>
    (
        this IFeatureSliceSetup<TRequest, TResult, TResponse> options,
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
                var dispatch = options.GetHandle(provider);
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

    public static IFeatureSliceSetup<TRequest, TResult, TResponse> MapJob<TRequest, TResult, TResponse>
    (
        this IFeatureSliceSetup<TRequest, TResult, TResponse> options,
        Func<bool> shouldRun,
        TRequest request
    )
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        return options.MapJob(shouldRun, () => request);
    }

    public static IFeatureSliceSetup<TRequest, TResult, TResponse> MapCronJob<TRequest, TResult, TResponse>
    (
        this IFeatureSliceSetup<TRequest, TResult, TResponse> options,
        string cronExpression,
        Func<TRequest> request
    )
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        var cron = CrontabSchedule.TryParse(cronExpression);

        var lastTime = DateTime.UtcNow;

        return options.MapJob(() => 
        {
            var timeNow = DateTime.UtcNow;
            var occurrences = cron.GetNextOccurrences(lastTime, timeNow).ToArray();

            lastTime = timeNow;

            return occurrences.Length > 0; 
        }
        , request);
   }

    public static IFeatureSliceSetup<TRequest, TResult, TResponse> MapCronJob<TRequest, TResult, TResponse>
    (
        this IFeatureSliceSetup<TRequest, TResult, TResponse> options,
        string cronExpression,
        TRequest request
    )
        where TRequest : notnull
        where TResult : Result_Base<TResponse>
        where TResponse : notnull
    {
        return options.MapCronJob(cronExpression, () => request);
    }
}