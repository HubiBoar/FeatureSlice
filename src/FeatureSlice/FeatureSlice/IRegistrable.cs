using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

public interface IRegistrable
{
    public abstract static void Register(IServiceCollection services);
}

public interface IRegistrable<TArgs>
{
    public abstract static void Register(TArgs args);
}

public static class RegistrableExtensions
{
    public static void Register<T>(this IServiceCollection services)
        where T : IRegistrable
    {
        T.Register(services);
    }

    public static void Register<T, TArgs>(this TArgs args)
        where T : IRegistrable<TArgs>
    {
        T.Register(args);
    }
}