using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice.Handle2;

public interface IDependencies<TSelf>
    where TSelf : IDependencies<TSelf>
{
    public abstract static TSelf Get(IServiceProvider provider); 
}

public sealed record Deps<T0>(T0 Dep0) : IDependencies<Deps<T0>>
    where T0 : notnull
{
    public static Deps<T0> Get(IServiceProvider provider)
    {
        return new (provider.GetRequiredService<T0>());
    }

}

public sealed record Deps<T0, T1>(T0 Dep0, T1 Dep1) : IDependencies<Deps<T0, T1>>
    where T0 : notnull
    where T1 : notnull
{
    public static Deps<T0, T1> Get(IServiceProvider provider)
    {
        return new
        (
            provider.GetRequiredService<T0>(),
            provider.GetRequiredService<T1>()
        );
    }
}

public interface IFeatureSliceSetup
{
    public void Configure(IServiceProvider provider);
}

public interface IFeatureSliceHtml
{
    string Route { get; }
    string Method { get; }
}

public sealed record FeatureSliceHtml(string Route, string Method) : IFeatureSliceHtml;

public interface IFeatureSliceDispatch<TRequest, TResponse> : IFeatureSliceSetup
{
    Func<TRequest, TResponse> Dispatch { get; set; }
}

public interface IFeatureSliceDispatch<TRequest, TResponse, TDeps>
: IFeatureSliceDispatch<TRequest, TResponse>
where TDeps: IDependencies<TDeps>
{
    public TResponse Handle(TRequest request, TDeps deps);
}

public sealed record FeatureSliceDispatch<TRequest, TResponse, TDeps>
(
    Func<TRequest, TDeps, TResponse> Method
)
: IFeatureSliceDispatch<TRequest, TResponse, TDeps>
where TDeps: IDependencies<TDeps>
{
    public Func<TRequest, TResponse> Dispatch { get; set; } = null!;

    public void Configure(IServiceProvider provider)
    {
        var deps = TDeps.Get(provider);

        Dispatch = request => Handle(request, deps); 
    }

    public TResponse Handle(TRequest request, TDeps deps) => Method(request, deps);
}

public abstract record FeatureSlice(IFeatureSliceSetup Setup)
{
    public static IFeatureSliceDispatch<TRequest, TResponse, Deps<TDep0, TDep1>> Handle<TRequest, TDep0, TDep1, TResponse>
    (
        Func<TRequest, TDep0, TDep1, TResponse> handle
    )
        where TDep0 : notnull
        where TDep1 : notnull
    {
        return new FeatureSliceDispatch<TRequest, TResponse, Deps<TDep0, TDep1>>
        (
            (request, deps) => handle(request, deps.Dep0, deps.Dep1)
        );
    }
}

public static class Extension
{
    public static IFeatureSliceHtml Html(string route, string method)
    {
        return new FeatureSliceHtml(route, method);
    }
}

public sealed record Dep0;
public sealed record Dep1;

public sealed partial record Example() : FeatureSlice
(
    Handle<Request, Dep0, Dep1, Response>(static (request, dep0, dep1) => 
    {
        return new Response();
    }) 
)
{
    public sealed record Request();
    public sealed record Response();
}

//Auto generated
public sealed partial record Example :
    IFeatureSliceDispatch<Example.Request, Example.Response, Deps<Dep0, Dep1>>
{
    public Func<Request, Response> Dispatch
    {
        get => ((IFeatureSliceDispatch<Example.Request, Example.Response, Deps<Dep0, Dep1>>)Setup).Dispatch; 
        set => ((IFeatureSliceDispatch<Example.Request, Example.Response, Deps<Dep0, Dep1>>)Setup).Dispatch = value;
    }

    public void Configure(IServiceProvider provider)
    {
        ((IFeatureSliceDispatch<Example.Request, Example.Response, Deps<Dep0, Dep1>>)Setup).Configure(provider);
    }

    public Response Handle(Request request, Deps<Dep0, Dep1> deps)
    {
        return ((IFeatureSliceDispatch<Example.Request, Example.Response, Deps<Dep0, Dep1>>)Setup).Handle(request, deps);
    }
}
