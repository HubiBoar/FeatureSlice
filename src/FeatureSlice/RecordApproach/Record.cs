using Definit.Results;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

public sealed record FeatureSliceSetup<TRequest, TResponse>(Action<IServiceCollection> provider)
{
}

public abstract record FeatureSliceRecordBase<TSelf, TRequest, TResult, TResponse>
(
    FeatureSliceBase<TRequest, TResult, TResponse>.ISetup Setup
)
    where TSelf : FeatureSliceRecordBase<TSelf, TRequest, TResult, TResponse>, new()
    where TRequest : notnull
    where TResult : Result_Base<TResponse>
    where TResponse : notnull
{
    public delegate Dispatch DispatchFactory(IServiceProvider provider);
    public delegate Task<TResponse> Dispatch(TRequest request);

    public static FeatureSliceBase<TRequest, TResult, TResponse>.ISetup Handle(Func<TRequest, Task<TResponse>> func)
    {
        return provider => request => func(request);
    }

    public static FeatureSliceBase<TRequest, TResult, TResponse>.ISetup Handle(Func<TRequest, TResponse> func)
    {
        return provider => request => Task.FromResult(func(request));
    }

    public static FeatureSliceBase<TRequest, TResult, TResponse>.ISetup Handle<TDep0>(Func<TRequest, TDep0, Task<TResponse>> func)
        where TDep0 : notnull
    {
        return provider => request => func(request, provider.GetRequiredService<TDep0>());
    }

    public static FeatureSliceBase<TRequest, TResult, TResponse>.ISetup Handle<TDep0, TDep1>(Func<TRequest, TDep0, TDep1, Task<TResponse>> func)
        where TDep0 : notnull
    {
        return provider => request => func(request, provider.GetRequiredService<TDep0>());
    }

    public static FeatureSliceBase<TRequest, TResult, TResponse>.ISetup Handle<TDep0>(Func<TRequest, TDep0, TResponse> func)
        where TDep0 : notnull
    {
        return provider => request => Task.FromResult(func(request, provider.GetRequiredService<TDep0>()));
    }
}

public abstract record FeatureSliceRecord<TSelf, TRequest, TResponse>
(
    FeatureSliceBase<TRequest, Result<TResponse>, TResponse>.ISetup Setup
)
: FeatureSliceRecordBase<TSelf, TRequest, Result<TResponse>, TResponse>(Setup)

    where TSelf : FeatureSliceRecord<TSelf, TRequest, TResponse>, new()
    where TRequest : notnull
    where TResponse : notnull;

public sealed record TestDependency();

public sealed record TestFeatureSlice()
    : FeatureSliceRecord<TestFeatureSlice, TestFeatureSlice.Request, TestFeatureSlice.Response>
    (
        Handle((Request request, TestDependency test) => 
        {
            return new Response();
        })
        .MapPost("/test")
    )
{
    public sealed record Request();
    public sealed record Response();
}

public static class TestConvert
{
    public static FeatureSliceSetup<TRequest, TResponse> MapPost<TRequest, TResponse>
    (
        this FeatureSliceSetup<TRequest, TResponse> factory,
        string route
    )
    {
        return factory;
    }
}