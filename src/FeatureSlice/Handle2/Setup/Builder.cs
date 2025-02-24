namespace FeatureSlice.Handle2;

public interface IFeatureSliceBuilder
{
    public IReadOnlyCollection<IFeatureSliceSetup> Setups { get; }

    public interface IWithMetadata<T> :IFeatureSliceBuilder
    {
        public T Metadata { get; }
    }
}

public interface IFeatureSliceBuilder<T> : IFeatureSliceBuilder
    where T : IFeatureSliceSetup
{
    public T Setup { get; }
}

public interface IFeatureSliceBuilder<T, TMetadata> : IFeatureSliceBuilder<T>, IFeatureSliceBuilder.IWithMetadata<TMetadata>
    where T : IFeatureSliceSetup
{
}

public sealed record FeatureSliceBuilder<T, TMetadata> : IFeatureSliceBuilder<T, TMetadata>
    where T : IFeatureSliceSetup
{
    public IReadOnlyCollection<IFeatureSliceSetup> Setups => _setups;

    public TMetadata Metadata { get; }

    public T Setup { get; }

    private readonly List<IFeatureSliceSetup> _setups = [];

    public FeatureSliceBuilder(T setup, TMetadata metadata)
    {
        _setups.Add(setup);
        Setup = setup;
        this.Metadata = metadata;
    }

    public FeatureSliceBuilder(IFeatureSliceBuilder builder, T setup, TMetadata metadata)
    {
        _setups.AddRange(builder.Setups);
        _setups.Add(setup);
        Setup = setup;
        Metadata = metadata;
    }

    public FeatureSliceBuilder(IFeatureSliceBuilder.IWithMetadata<TMetadata> builder, T setup)
    {
        _setups.AddRange(builder.Setups);
        _setups.Add(setup);
        Setup = setup;
        Metadata = builder.Metadata;
    }
}
