using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

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
        return new FromServices<T0>(
            provider.GetRequiredService<T0>());
    }

    public void Deconstruct(out T0 value0)
    {
        value0 = Value0;
    }
}

public class FromServiceProvider
{
    public IServiceProvider Provider {get;}

    public FromServiceProvider(IServiceProvider provider)
    {
        Provider = provider;
    }
}

public static class ServiceProviderExtensions
{
    public static FromServiceProvider From(this IServiceProvider provider)
    {
        return new FromServiceProvider(provider);
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
        return new FromServices<T0, T1>(
            provider.GetRequiredService<T0>(),
            provider.GetRequiredService<T1>());
    }

    public static implicit operator FromServices<T0, T1>(FromServiceProvider provider)
    {
        return Create(provider.Provider);
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
        return new FromServices<T0, T1, T2>(
            provider.GetRequiredService<T0>(),
            provider.GetRequiredService<T1>(),
            provider.GetRequiredService<T2>());
    }

    public void Deconstruct(
        out T0 value0,
        out T1 value1,
        out T2 value2)
    {
        value0 = Value0;
        value1 = Value1;
        value2 = Value2;
    }
}

public sealed class FromServices<T0, T1, T2, T3> : IFromServices<FromServices<T0, T1, T2, T3>>
    where T0 : class
    where T1 : class
    where T2 : class
    where T3 : class
{
    public static Type[] Types { get; } = [ typeof(T0), typeof(T1), typeof(T2) , typeof(T3) ];

    public T0 Value0 { get; }

    public T1 Value1 { get; }

    public T2 Value2 { get; }

    public T3 Value3 { get; }

    public FromServices(T0 value0, T1 value1, T2 value2, T3 value3)
    {
        Value0 = value0;
        Value1 = value1;
        Value2 = value2;
        Value3 = value3;
    }

    public static FromServices<T0, T1, T2, T3> Create(IServiceProvider provider)
    {
        return new FromServices<T0, T1, T2, T3>(
            provider.GetRequiredService<T0>(),
            provider.GetRequiredService<T1>(),
            provider.GetRequiredService<T2>(),
            provider.GetRequiredService<T3>());
    }

    public void Deconstruct(
        out T0 value0,
        out T1 value1,
        out T2 value2,
        out T3 value3)
    {
        value0 = Value0;
        value1 = Value1;
        value2 = Value2;
        value3 = Value3;
    }
}

public sealed class FromServices<T0, T1, T2, T3, T4> : IFromServices<FromServices<T0, T1, T2, T3, T4>>
    where T0 : class
    where T1 : class
    where T2 : class
    where T3 : class
    where T4 : class
{
    public static Type[] Types { get; } = [ typeof(T0), typeof(T1), typeof(T2) , typeof(T3) , typeof(T4) ];

    public T0 Value0 { get; }

    public T1 Value1 { get; }

    public T2 Value2 { get; }

    public T3 Value3 { get; }

    public T4 Value4 { get; }

    public FromServices(T0 value0, T1 value1, T2 value2, T3 value3, T4 value4)
    {
        Value0 = value0;
        Value1 = value1;
        Value2 = value2;
        Value3 = value3;
        Value4 = value4;
    }

    public static FromServices<T0, T1, T2, T3, T4> Create(IServiceProvider provider)
    {
        return new FromServices<T0, T1, T2, T3, T4>(
            provider.GetRequiredService<T0>(),
            provider.GetRequiredService<T1>(),
            provider.GetRequiredService<T2>(),
            provider.GetRequiredService<T3>(),
            provider.GetRequiredService<T4>());
    }

    public void Deconstruct(
        out T0 value0,
        out T1 value1,
        out T2 value2,
        out T3 value3,
        out T4 value4)
    {
        value0 = Value0;
        value1 = Value1;
        value2 = Value2;
        value3 = Value3;
        value4 = Value4;
    }
}

public sealed class FromServices<T0, T1, T2, T3, T4, T5> : IFromServices<FromServices<T0, T1, T2, T3, T4, T5>>
    where T0 : class
    where T1 : class
    where T2 : class
    where T3 : class
    where T4 : class
    where T5 : class
{
    public static Type[] Types { get; } = [ typeof(T0), typeof(T1), typeof(T2) , typeof(T3) , typeof(T4) , typeof(T5) ];

    public T0 Value0 { get; }

    public T1 Value1 { get; }

    public T2 Value2 { get; }

    public T3 Value3 { get; }

    public T4 Value4 { get; }

    public T5 Value5 { get; }

    public FromServices(T0 value0, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
    {
        Value0 = value0;
        Value1 = value1;
        Value2 = value2;
        Value3 = value3;
        Value4 = value4;
        Value5 = value5;
    }

    public static FromServices<T0, T1, T2, T3, T4, T5> Create(IServiceProvider provider)
    {
        return new FromServices<T0, T1, T2, T3, T4, T5>(
            provider.GetRequiredService<T0>(),
            provider.GetRequiredService<T1>(),
            provider.GetRequiredService<T2>(),
            provider.GetRequiredService<T3>(),
            provider.GetRequiredService<T4>(),
            provider.GetRequiredService<T5>());
    }

    public void Deconstruct(
        out T0 value0,
        out T1 value1,
        out T2 value2,
        out T3 value3,
        out T4 value4,
        out T5 value5)
    {
        value0 = Value0;
        value1 = Value1;
        value2 = Value2;
        value3 = Value3;
        value4 = Value4;
        value5 = Value5;
    }
}