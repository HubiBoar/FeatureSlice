namespace FeatureSlice.FluentGenerics.Interfaces2;

public struct Disabled;

public interface IFeatureFlag
{
    public abstract static string FeatureName { get; }
}