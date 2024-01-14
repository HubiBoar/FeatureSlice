namespace OneOf2Example;

public sealed partial record T1Example()
{
    public Task<ExampleReturn> Handle()
    {
        return Task.FromResult(new ExampleReturn());
    }
}

public sealed partial record T2Example()
{
    public Task<ExampleReturn> Handle()
    {
        return Task.FromResult(new ExampleReturn());
    }
}

public sealed partial record T3Example()
{
    public Task<ExampleReturn> Handle()
    {
        return Task.FromResult(new ExampleReturn());
    }
}

public sealed record ExampleReturn();

public interface IOneOfT0<T0>
{
}
public interface IOneOfT1<T1>
{
}
public interface IOneOfT2<T2>
{
}

public interface IOneOf<T0, T1> : IOneOfT0<T0>, IOneOfT1<T1>
{
    public class Get : IOneOf<T0, T1>
    {
        public static implicit operator Get(T0 value)
        {
            return new Get();
        }

        public static implicit operator Get(T1 value)
        {
            return new Get();
        }
    }
}

public interface IOneOf<T0, T1, T2> : IOneOfT0<T0>, IOneOfT1<T1>, IOneOfT2<T2>
{
    public class Get : IOneOf<T0, T1, T2>
    {
        public static implicit operator Get(T0 value)
        {
            return new Get();
        }

        public static implicit operator Get(T1 value)
        {
            return new Get();
        }

        public static implicit operator Get(T2 value)
        {
            return new Get();
        }
    }
}


public sealed class Dependncy
{
    public IOneOf<T1Example, T2Example>.Get Handle(IOneOf<T1Example, T2Example, T3Example> oneOf)
    {
        return oneOf switch
        {
            T1Example t => t,
            T2Example t => t,
            T3Example t => throw new Exception(),
            _ => throw new Exception()
        };
    }

    public Task<ExampleReturn> HandleAsync(IOneOf<T2Example, T2Example, T3Example> oneOf)
    {
        //Does not work
        return oneOf switch
        {
            T1Example t => t.Handle(),
            T2Example t => t.Handle(),
            T3Example t => t.Handle(),
            _ => throw new Exception()
        };
    }
}


//Auto Generated
public partial record T1Example : IOneOf<T1Example, T2Example, T3Example>, IOneOf<T1Example, T1Example, T3Example>
{
}

//Auto Generated
public partial record T2Example : IOneOf<T1Example, T2Example, T3Example>, IOneOf<T2Example, T2Example, T3Example>
{
}

//Auto Generated
public partial record T3Example : IOneOf<T1Example, T2Example, T3Example>, IOneOf<T2Example, T2Example, T3Example>
{
}