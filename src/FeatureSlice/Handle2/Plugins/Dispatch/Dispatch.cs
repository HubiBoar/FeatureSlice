namespace FeatureSlice.Handle2;

public interface IFeatureSliceHandle<TRequest, TResponse, TDeps> : IFeatureSliceSetup
where TDeps: IDependencies<TDeps>
{
    public TResponse Handle(TRequest request, TDeps deps);
}

public sealed record FeatureSliceHandle<TRequest, TResponse, TDeps>
(
    Func<TRequest, TDeps, TResponse> Method
)
: IFeatureSliceHandle<TRequest, TResponse, TDeps>
where TDeps: IDependencies<TDeps>
{
    public void Configure(IServiceProvider provider)
    {
    }

    public TResponse HandleHelper(IServiceProvider provider, TRequest request)
    {
        var deps = TDeps.Get(provider);

        return Handle(request, deps); 
    }

    public TResponse Handle(TRequest request, TDeps deps) => Method(request, deps);
}

public abstract partial record FeatureSlice<TRequest, TResponse>
{
    public static FeatureSlice<TRequest, TResponse>.Config<IFeatureSliceHandle<TRequest, TResponse, Deps<TDep0, TDep1>>> Handle<TDep0, TDep1>
    (
        Func<TRequest, TDep0, TDep1, TResponse> handle
    )
        where TDep0 : notnull
        where TDep1 : notnull
    {
        var dispatch = new FeatureSliceHandle<TRequest, TResponse, Deps<TDep0, TDep1>>
        (
            (request, deps) => handle(request, deps.Dep0, deps.Dep1)
        );

        var builder = new FeatureSlice<TRequest, TResponse>.Builder(dispatch.HandleHelper);

        return new (dispatch, builder);
    }
}
