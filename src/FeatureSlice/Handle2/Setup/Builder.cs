namespace FeatureSlice.Handle2;


public interface IFeatureSliceConfig
{
    public interface IWithMetadata<T> : IFeatureSliceConfig
    {
        public T Metadata { get; }
    }

    public interface IWithSetup<T> : IFeatureSliceConfig
        where T : IFeatureSliceSetup
    {
        public T Setup { get; }
    }

    public interface IDispatch<TRequest, TResponse> : IRequest<TRequest>, IResponse<TResponse>
    {
        public TResponse Dispatch(IServiceProvider provider, TRequest request); 

        public new interface IWithSetup<T> :
            IFeatureSliceConfig.IWithSetup<T>,
            IFeatureSliceConfig.IDispatch<TRequest, TResponse>
            where T : IFeatureSliceSetup
        {
            public new interface IWithMetadata<TMetadata> :
                IFeatureSliceConfig.IDispatch<TRequest, TResponse>.IWithMetadata<TMetadata>,
                IFeatureSliceConfig.IDispatch<TRequest, TResponse>.IWithSetup<T>;
        }

        public new interface IWithMetadata<T> :
            IFeatureSliceConfig.IWithMetadata<T>,
            IFeatureSliceConfig.IDispatch<TRequest, TResponse>;
    }

    public interface IRequest<TRequest> : IFeatureSliceConfig
    {
    }

    public interface IResponse<TResponse> : IFeatureSliceConfig
    {
    }
}

//public sealed record FeatureSliceBuilder<TSetup, TRequest, TResponse, TMetadata> : IFeatureSliceBuilder
//    .IDispatch<TRequest, TResponse>
//    .IWithSetup<TSetup>
//    .IWithMetadata<TMetadata>
//    where TSetup : IFeatureSliceSetup
//{
//    public IReadOnlyCollection<IFeatureSliceSetup> Setups => _setups;
//
//    public TMetadata Metadata { get; }
//
//    public TSetup Setup { get; }
//
//    internal Func<IServiceProvider, TRequest, TResponse> Dispatch { get; } 
//
//    private readonly List<IFeatureSliceSetup> _setups = [];
//
//    public FeatureSliceBuilder(TSetup setup, TMetadata metadata, Func<IServiceProvider, TRequest, TResponse> dispatch)
//    {
//        _setups.Add(setup);
//        Setup = setup;
//        this.Metadata = metadata;
//        Dispatch = dispatch;
//    }
//
//    public FeatureSliceBuilder(IFeatureSliceBuilder builder, TSetup setup, TMetadata metadata)
//    {
//        _setups.AddRange(builder.Setups);
//        _setups.Add(setup);
//        Setup = setup;
//        Metadata = metadata;
//    }
//
//    public FeatureSliceBuilder(IFeatureSliceBuilder.IWithMetadata<TMetadata> builder, TSetup setup)
//    {
//        _setups.AddRange(builder.Setups);
//        _setups.Add(setup);
//        Setup = setup;
//        Metadata = builder.Metadata;
//    }
//}
