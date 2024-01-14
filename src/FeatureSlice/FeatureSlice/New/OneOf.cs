using OneOfGen;

namespace OneOfExample;

public interface IOneOfT0<T>
    where T : IOneOfElement
{
}
public interface IOneOfT1<T>
    where T : IOneOfElement
{
}
public interface IOneOfT2<T>
    where T : IOneOfElement
{
}

public interface IOneOf<T0, T1> : IOneOfT0<T0>, IOneOfT1<T1>
    where T0 : IOneOfElement
    where T1 : IOneOfElement
{
}

public interface IOneOf<T0, T1, T2> : IOneOfT0<T0>, IOneOfT1<T1>, IOneOfT2<T2>
    where T0 : IOneOfElement
    where T1 : IOneOfElement
    where T2 : IOneOfElement
{
}