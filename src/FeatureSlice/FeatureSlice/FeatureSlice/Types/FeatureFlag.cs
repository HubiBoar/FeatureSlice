namespace FeatureSlice;

public struct Disabled;

public interface IFeatureFlag
{
    public abstract static string FeatureName { get; }
}