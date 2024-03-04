using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

public static class DependencyInjectionExtensions
{
    public static void Add<TService>(
        this IServiceCollection services,
        ServiceLifetime lifetime,
        Func<IServiceProvider, TService> factory)
        where TService : notnull
    {
        services.Add(ServiceDescriptor.Describe(typeof(TService), provider => factory(provider), lifetime));
    }

    public static FluentFeatureSlice.IInitial FeatureSlice(this IServiceCollection services)
    {
        return FluentFeatureSlice.Create(services);
    }
}