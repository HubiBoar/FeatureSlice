using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;

namespace FeatureSlice;

public interface IStaticHandler<TRequest, TResponse, TDependencies>
    where TDependencies : class, IFromServices<TDependencies>
{
    public virtual static Task<OneOf<TResponse, Error>> Dispatch<THandler>(TRequest request, IServiceProvider provider)
        where THandler : IStaticHandler<TRequest, TResponse, TDependencies>
    {
        return THandler.Handle(request, TDependencies.Create(provider));
    }

    public static abstract Task<OneOf<TResponse, Error>> Handle(TRequest request, TDependencies dependencies);
}

public interface IFromServices<TSelf>
    where TSelf : class, IFromServices<TSelf>
{
    public abstract static Type[] Types { get; }

    public static abstract TSelf Create(IServiceProvider provider);
}

public sealed class FromServices<T0> : IFromServices<FromServices<T0>>
    where T0 : class
{
    public static Type[] Types { get; } = [ typeof(T0) ];

    public T0 Value0 { get; }

    public FromServices(T0 value0)
    {
        Value0 = value0;
    }

    public static FromServices<T0> Create(IServiceProvider provider)
    {
        return new FromServices<T0>(provider.GetRequiredService<T0>());
    }

    public void Deconstruct(out T0 value0)
    {
        value0 = Value0;
    }
}

public sealed class FromServices<T0, T1> : IFromServices<FromServices<T0, T1>>
    where T0 : class
    where T1 : class
{
    public static Type[] Types { get; } = [ typeof(T0), typeof(T1) ];

    public T0 Value0 { get; }

    public T1 Value1 { get; }

    public FromServices(T0 value0, T1 value1)
    {
        Value0 = value0;
        Value1 = value1;
    }

    public static FromServices<T0, T1> Create(IServiceProvider provider)
    {
        return new FromServices<T0, T1>(provider.GetRequiredService<T0>(), provider.GetRequiredService<T1>());
    }

    public void Deconstruct(out T0 value0, out T1 value1)
    {
        value0 = Value0;
        value1 = Value1;
    }
}

public sealed class FromServices<T0, T1, T2> : IFromServices<FromServices<T0, T1, T2>>
    where T0 : class
    where T1 : class
    where T2 : class
{
    public static Type[] Types { get; } = [ typeof(T0), typeof(T1), typeof(T2) ];

    public T0 Value0 { get; }

    public T1 Value1 { get; }

    public T2 Value2 { get; }

    public FromServices(T0 value0, T1 value1, T2 value2)
    {
        Value0 = value0;
        Value1 = value1;
        Value2 = value2;
    }

    public static FromServices<T0, T1, T2> Create(IServiceProvider provider)
    {
        return new FromServices<T0, T1, T2>(provider.GetRequiredService<T0>(), provider.GetRequiredService<T1>(), provider.GetRequiredService<T2>());
    }

    public void Deconstruct(out T0 value0, out T1 value1, out T2 value2)
    {
        value0 = Value0;
        value1 = Value1;
        value2 = Value2;
    }
}