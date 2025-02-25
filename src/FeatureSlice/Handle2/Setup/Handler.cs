using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice.Handle2;

public interface IFeatureSliceBase
{
    public interface IRequest<TRequest> : IFeatureSliceBase;
    public interface IResponse<TResponse> : IFeatureSliceBase;

    public interface IDispatch<TRequest, TResponse> : IRequest<TRequest>, IResponse<TResponse>
    {
        public Func<TRequest, TResponse> Dispatch { get; init; } 
    }
}

public abstract partial record FeatureSlice<TRequest, TResponse>(FeatureSlice<TRequest, TResponse>.IConfig Configuration)
:
    IFeatureSliceBase.IDispatch<TRequest, TResponse>
{
    public required Func<TRequest, TResponse> Dispatch { get; init; }

    public sealed class Builder
    {
        public Func<IServiceProvider, TRequest, TResponse> Dispatch { get; }

        private readonly List<IFeatureSliceSetup> _setups;

        internal Builder(Func<IServiceProvider, TRequest, TResponse> dispatch)
        {
            Dispatch = dispatch;

            _setups = [];
        }

        public T? TryGetSetup<T>()
            where T : IFeatureSliceSetup
        {
            return _setups.OfType<T>().FirstOrDefault();
        }

        public void Configure(IServiceProvider provider)
        {
            foreach (var setup in _setups)
            {
                setup.Configure(provider);
            }
        }

        public void AddSetup(IFeatureSliceSetup setup)
        {
            _setups.Add(setup);
        }
    }

    public sealed record Config<TSetup, TMetadata>
    (
        TSetup Setup,
        TMetadata Metadata,
        Builder Builder
    )
    : IConfig.ISetup<TSetup>.IMetadata<TMetadata>
        where TSetup : IFeatureSliceSetup;

    public sealed record Config<TSetup>
    (
        TSetup Setup,
        Builder Builder
    )
    : IConfig.ISetup<TSetup>
        where TSetup : IFeatureSliceSetup;

    public interface IConfig
    {
        public Builder Builder { get; } 

        public interface ISetup<TSetup> : IConfig
            where TSetup : IFeatureSliceSetup
        {
            public TSetup Setup { get; }

            public interface IAndMetadata<TMetadata> : IConfig.ISetup<TSetup>, IConfig.IMetadata<TMetadata>
            {
            }
        }

        public interface IMetadata<TMetadata> : IConfig
        {
            public TMetadata Metadata { get; }
        }
    }

    protected T? TryGetSetup<T>()
        where T : IFeatureSliceSetup
    {
        return Configuration.Builder.TryGetSetup<T>();
    }
}
