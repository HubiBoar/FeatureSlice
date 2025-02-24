namespace FeatureSlice.Handle2;

public interface IFeatureSliceBase
{
    public interface IRequest<TRequest> : IFeatureSliceBase
    {
        public interface IResponse<TResponse> : IRequest<TRequest>
        {
        }
    }
}

public abstract partial record FeatureSlice<TRequest, TResponse>(
    IFeatureSliceBuilder Builder
    ) : IFeatureSliceBase.IRequest<TRequest>.IResponse<TResponse>
{
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
