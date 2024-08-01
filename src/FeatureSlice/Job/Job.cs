using Definit.Results;
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
            var jobs = _jobs.Select(job => Run(job, ct));

            await Task.WhenAll(jobs);

            await Task.Delay(1000, ct);
        }
    }

    private async Task Run(FeatureSliceJob job, CancellationToken ct)
    {
        try
        {
            if(job.ShouldRun() == false)
            {
                return;
            }

            await job.Job(ct);
        }
        catch
        {
            
        }
    }
}
