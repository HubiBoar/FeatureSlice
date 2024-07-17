using Definit.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FeatureSlice;

public sealed record FeatureSliceJob(Func<bool> ShouldRun, Func<CancellationToken, Task<Result>> Job);

public sealed class FeatureSliceJobRunner : BackgroundService
{
    private readonly IReadOnlyCollection<FeatureSliceJob> _jobs;

    public FeatureSliceJobRunner(IEnumerable<FeatureSliceJob> jobs)
    {
        _jobs = jobs.ToArray();
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while(ct.IsCancellationRequested == false)
        {
            var jobs = _jobs.Where(job => job.ShouldRun()).Select(job => Run(job, ct));

            await Task.WhenAll(jobs);
        }
    }

    private async Task Run(FeatureSliceJob job, CancellationToken ct)
    {
        try
        {
            await job.Job(ct);
        }
        catch
        {
            
        }
    }
}

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