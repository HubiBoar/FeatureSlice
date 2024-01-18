using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FeatureSlice.Dispatch;

public interface IRegistrable
{
    public static abstract void Register(IServiceCollection services);
}

public static class RegistrableExtensions
{
    public static IServiceCollection Register<T>(this IServiceCollection services)
        where T : IRegistrable
    {
        T.Register(services);

        return services;
    }
}

public delegate void HostExtesnion<THost>(THost extension)
    where THost : IHost;

public static class ServiceCollectionExtensions
{
    public static void AddHostExtension<THost>(this IServiceCollection services, HostExtesnion<THost> extension)
        where THost : IHost
    {
        services.AddSingleton(extension);
    }

    public static void Extend<THost>(this THost host)
        where THost : IHost
    {
        foreach(var extension in host.Services.GetServices<HostExtesnion<THost>>())
        {
            extension.Invoke(host);
        }

        foreach(var extension in host.Services.GetServices<HostExtesnion<IHost>>())
        {
            extension.Invoke(host);
        }
    }
}