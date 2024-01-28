using Microsoft.FeatureManagement;

namespace FeatureSlice;

public interface IFeatureName
{
    public static abstract string FeatureName { get; }
}

public static class FeatureExtensions
{
    public static Task<bool> IsEnabledAsync<T>(this IFeatureManager featureManager)
        where T : IFeatureName
    {
        return featureManager.IsEnabledAsync(T.FeatureName);
    }

    public static Task<bool> IsEnabledAsync<T>(this IFeatureManager featureManager, T featureName)
        where T : IFeatureName
    {
        return featureManager.IsEnabledAsync(T.FeatureName);
    }
}