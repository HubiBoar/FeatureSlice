namespace FeatureSlice.Handle2;

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

public abstract partial record FeatureSlice<TRequest, TResponse>
{
    public static IFeatureSliceBuilder<IFeatureSliceDispatch<TRequest, TResponse, Deps<TDep0, TDep1>>> Handle<TDep0, TDep1>
    (
        Func<TRequest, TDep0, TDep1, TResponse> handle
    )
        where TDep0 : notnull
        where TDep1 : notnull
    {
        var dispatch = new FeatureSliceDispatch<TRequest, TResponse, Deps<TDep0, TDep1>>
        (
            (request, deps) => handle(request, deps.Dep0, deps.Dep1)
        );

        return new FeatureSliceBuilder<IFeatureSliceDispatch<TRequest, TResponse, Deps<TDep0, TDep1>>, IFeatureSliceSetup>
        (
            dispatch,
            dispatch
        );
    }
}
