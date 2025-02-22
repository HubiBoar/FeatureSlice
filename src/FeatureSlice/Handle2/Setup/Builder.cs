namespace FeatureSlice.Handle2;

public interface IFeatureSliceBuilder
{
    public IReadOnlyCollection<IFeatureSliceSetup> Setups { get; }
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
