using Definit.Results;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice.New;

public interface IFeatureSlice
{
    protected void Configure(IServiceProvider provider);

    public static void Register<T>(IServiceCollection services)
        where T : class, IFeatureSlice, new()
    {
        services.AddTransient(provider =>
        {
            var slice = new T();

            slice.Configure(provider);

            return slice;
        });
    }
}

public interface IFeatureSlice<TDelegate> : IFeatureSlice
    where TDelegate : Delegate
{
    public TDelegate Dispatch { get; protected set; }

    protected Options Opts { get; }

    public sealed record Options(Func<IServiceProvider, TDelegate> GetHandle);

    void IFeatureSlice.Configure(IServiceProvider provider)
    {
        Dispatch = Opts.GetHandle(provider);
    }

    public static T CreateForTest<T>(TDelegate handle)
        where T : class, IFeatureSlice<TDelegate>, new()
    {
        return new T()
        {
            Dispatch = handle
        };
    }
}

public abstract record FeatureSliceBase<TRequest, TResult, TResponse>
(
    IFeatureSlice<FeatureSliceBase<TRequest, TResult, TResponse>.HandleDelegate>.Options Opts
)
: IFeatureSlice<FeatureSliceBase<TRequest, TResult, TResponse>.HandleDelegate>

where TRequest : notnull
where TResult : Result_Base<TResponse>
where TResponse : notnull

{
    public delegate Task<TResult> HandleDelegate(TRequest request);

    public HandleDelegate Dispatch { get; private set; } = null!;

    HandleDelegate IFeatureSlice<HandleDelegate>.Dispatch
    {
        get => Dispatch;
        set => Dispatch = value;
    }

    public static IFeatureSlice<HandleDelegate>.Options Handle(HandleDelegate handle)
    {
        return new IFeatureSlice<HandleDelegate>.Options(_ => handle);
    }

    public static T CreateForTest<T>
    (
        HandleDelegate handle
    )
        where T : FeatureSliceBase<TRequest, TResult, TResponse>, new()
    {
        return IFeatureSlice<HandleDelegate>.CreateForTest<T>(handle);
    }
}

public abstract record FeatureSlice<TRequest, TResponse>
(
    IFeatureSlice<FeatureSlice<TRequest, TResponse>.HandleDelegate>.Options Opts
)
: FeatureSliceBase<TRequest, Result<TResponse>, TResponse>(Opts)

where TRequest : notnull
where TResponse : notnull
{
}

public static class FeatureSliceHelper
{
    public static void Register<T>(this IServiceCollection services)
        where T : class, IFeatureSlice, new()
    {
        IFeatureSlice.Register<T>(services);
    }
}


public sealed record ExampleHandler() : FeatureSlice<ExampleHandler.Request, ExampleHandler.Response>
(
    Handle(static async (Request request) =>
    {
        Console.WriteLine($"Handler: {request}");

        await Task.CompletedTask;

        return new Response();
    })
)
{
    public sealed record Request();

    public sealed record Response();
}

public static class Usage
{
    public static async Task<Result> Use(ExampleHandler handler)
    {
        return await handler.Dispatch(new ExampleHandler.Request());
    }

    public static Task<Result> Test()
    {
        var handler = ExampleHandler.CreateForTest<ExampleHandler>(async request => new ExampleHandler.Response());

        return Use(handler);
    }

    public static void Register(IServiceCollection services)
    {
        services.Register<ExampleHandler>();
    }
}