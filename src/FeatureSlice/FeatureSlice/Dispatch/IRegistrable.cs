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