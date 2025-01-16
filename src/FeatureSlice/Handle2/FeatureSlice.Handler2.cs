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

public sealed record FeatureSliceBuilder<T> : IFeatureSliceBuilder
    where T : IFeatureSliceSetup
{
    public IReadOnlyCollection<IFeatureSliceSetup> Setups => _setups;

    private readonly List<IFeatureSliceSetup> _setups = [];

    public FeatureSliceBuilder(T setup)
    {
        _setups.Add(setup);
    }

    public FeatureSliceBuilder(IFeatureSliceBuilder builder, T setup)
    {
        _setups.AddRange(builder.Setups);
        _setups.Add(setup);
    }
}

public interface IFeatureSliceBuilder
{
    public IReadOnlyCollection<IFeatureSliceSetup> Setups { get; }
}

public interface IFeatureSliceSetup
{
    public void Configure(IServiceProvider provider);
}

public sealed record Html(string Value)
{
    public static Html Empty { get; } = new Html(string.Empty);
}

public interface IFeatureSliceHtml : IFeatureSliceSetup
{
    string Route { get; }
    string Method { get; }
}

public interface IFeatureSliceHtml<TRequest> : IFeatureSliceHtml
{
    Html GetHtml(TRequest request);
}

public sealed record FeatureSliceHtml<TRequest>(string Route, string Method, Func<TRequest, Html> Get) : IFeatureSliceHtml<TRequest>
{
    public void Configure(IServiceProvider provider) {}

    public Html GetHtml(TRequest request) => Get(request);
}

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

public abstract record FeatureSlice(IFeatureSliceBuilder Builder)
{
    public static FeatureSliceBuilder<IFeatureSliceDispatch<TRequest, TResponse, Deps<TDep0, TDep1>>> Handle<TRequest, TDep0, TDep1, TResponse>
    (
        Func<TRequest, TDep0, TDep1, TResponse> handle
    )
        where TDep0 : notnull
        where TDep1 : notnull
    {
        return new FeatureSliceBuilder<IFeatureSliceDispatch<TRequest, TResponse, Deps<TDep0, TDep1>>>
        (
            new FeatureSliceDispatch<TRequest, TResponse, Deps<TDep0, TDep1>>
            (
                (request, deps) => handle(request, deps.Dep0, deps.Dep1)
            )
        );
    }

    protected T? TryGetSetup<T>()
        where T : IFeatureSliceSetup
    {
        return Builder.Setups.OfType<T>().FirstOrDefault();
    }

    public void Configure(IServiceProvider provider)
    {
        foreach (var setup in Builder.Setups)
        {
            setup.Configure(provider);
        }
    }
}

public static class Extension
{
    public static FeatureSliceBuilder<IFeatureSliceHtml<TRequest>> Html<TRequest>
    (
        this IFeatureSliceBuilder builder,
        string method, 
        string route,
        Func<TRequest, Html> get
    )
    {
        return new FeatureSliceBuilder<IFeatureSliceHtml<TRequest>>(builder, new FeatureSliceHtml<TRequest>(route, method, get));
    }

    public static Html Call<T>(this Html html)
        where T : IFeatureSliceHtml, new()
    {
        return html;
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
    .Html<Request>("Get", "/route", request => Html.Empty.Call<Example>())
)
{
    public sealed record Request();
    public sealed record Response();
}

//Auto generated
public sealed partial record Example :
    IFeatureSliceDispatch<Example.Request, Example.Response, Deps<Dep0, Dep1>>,
    IFeatureSliceHtml
{
    public Func<Request, Response> Dispatch
    {
        get => TryGetSetup<IFeatureSliceDispatch<Example.Request, Example.Response, Deps<Dep0, Dep1>>>()!.Dispatch; 
        set => TryGetSetup<IFeatureSliceDispatch<Example.Request, Example.Response, Deps<Dep0, Dep1>>>()!.Dispatch = value;
    }

    public string Route => TryGetSetup<IFeatureSliceHtml>()!.Route;

    public string Method => TryGetSetup<IFeatureSliceHtml>()!.Method;

    public Response Handle(Request request, Deps<Dep0, Dep1> deps)
    {
        return TryGetSetup<IFeatureSliceDispatch<Example.Request, Example.Response, Deps<Dep0, Dep1>>>()!.Handle(request, deps);
    }
}
