using OneOfGen;

namespace OneOf3Example;


public class OneOf<T0, T1>
    where T0 : IOneOfElement
    where T1 : IOneOfElement
{
    public class IsT0 : OneOf<T0, T1>
    {
        public T0 Value {get;}
    } 

    public class IsT1 : OneOf<T0, T1>
    {
        public T1 Value {get;}
    } 

    public static implicit operator OneOf<T0, T1>(T0 value)
    {
        return new OneOf<T0, T1>();
    }

    public static implicit operator OneOf<T0, T1>(T1 value)
    {
        return new OneOf<T0, T1>();
    }

    public static implicit operator T0(OneOf<T0, T1> value)
    {
        return new OneOf<T0, T1>();
    }

    public static implicit operator T1(OneOf<T0, T1> value)
    {
        return new OneOf<T0, T1>();
    }
}

public record T1Example() : IOneOfElement
{
    public Task Handle()
    {
        return Task.CompletedTask;
    }
}
public record T2Example() : IOneOfElement
{
    public Task Handle()
    {
        return Task.CompletedTask;
    }
}

public sealed class Dependncy
{
    public async Task Handle(OneOf<T1Example, T2Example> oneOf)
    {
        var task = oneOf switch
        {
            OneOf<T1Example, T2Example>.IsT0 t => t.Value.Handle(),
            OneOf<T1Example, T2Example>.IsT1 t => t.Value.Handle(),
            _ => Task.CompletedTask
        };

        await task;
    }
}