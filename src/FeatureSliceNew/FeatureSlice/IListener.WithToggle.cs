using Microsoft.FeatureManagement;

namespace FeatureSlice;

public partial interface IListener<TRequest>
{
    public interface WithToggle : IListener<TRequest>, IFeatureName
    {
        async Task IListener<TRequest>.HandleListener(TRequest request, IFeatureManager featureManager)
        {
            if(await IsEnabled(featureManager))
            {
                await Handle(request);
            }
        }

        protected Task<bool> IsEnabled(IFeatureManager featureManager);
    }
}