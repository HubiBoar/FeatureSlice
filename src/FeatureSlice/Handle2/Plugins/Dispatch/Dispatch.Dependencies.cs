using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice.Handle2;

public interface IDependencies<TSelf>
    where TSelf : IDependencies<TSelf>
{
    public abstract static TSelf Get(IServiceProvider provider); 
}

public sealed record Deps<T0>(T0 Dep0) : IDependencies<Deps<T0>>
    where T0 : notnull
{
    public static Deps<T0> Get(IServiceProvider provider)
    {
        return new (provider.GetRequiredService<T0>());
    }

}

public sealed record Deps<T0, T1>(T0 Dep0, T1 Dep1) : IDependencies<Deps<T0, T1>>
    where T0 : notnull
    where T1 : notnull
{
    public static Deps<T0, T1> Get(IServiceProvider provider)
    {
        return new
        (
            provider.GetRequiredService<T0>(),
            provider.GetRequiredService<T1>()
        );
    }
}
