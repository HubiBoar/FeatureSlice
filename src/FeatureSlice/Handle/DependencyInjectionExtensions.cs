using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

internal static class DependencyInjectionExtensions
{
    public static void Add<TService>(
        this IServiceCollection services,
        ServiceLifetime lifetime,
        Func<IServiceProvider, TService> factory)
        where TService : notnull
    {
        services.Add(ServiceDescriptor.Describe(typeof(TService), provider => factory(provider), lifetime));
    }
}
