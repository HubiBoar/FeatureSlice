using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice.Dispatch;

public interface IRegistrable
{
    public static abstract void Register(IServiceCollection services);
}