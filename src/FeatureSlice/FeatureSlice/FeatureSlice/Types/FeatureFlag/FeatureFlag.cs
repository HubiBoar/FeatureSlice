namespace FeatureSlice;

public struct Disabled;

public interface IFeatureFlag
{
    public abstract static string FeatureName { get; }
}

public static partial class FeatureSlice
{
    public static partial class AsFlag
    {
    }
}