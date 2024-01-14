namespace OneOfExample;

public record T1Example()
{
    public Task Handle()
    {
        return Task.CompletedTask;
    }
}
public record T2Example()
{
    public Task Handle()
    {
        return Task.CompletedTask;
    }
}
public record T3Example()
{
    public Task Handle()
    {
        return Task.CompletedTask;
    }
}

public interface IOneOf<T1, T2, T3>
{
}

public sealed record Is_T1Example(T1Example Value) : IOneOf<T1Example, T2Example, T3Example>, IOneOf<T3Example, T2Example, T1Example>
{
    public static explicit operator Is_T1Example(T1Example value)
    {
        return new Is_T1Example(value);
    }
}

public sealed record Is_T2Example(T2Example Value) : IOneOf<T1Example, T2Example, T3Example>, IOneOf<T3Example, T2Example, T1Example>
{
}

public sealed record Is_T3Example(T3Example Value) : IOneOf<T1Example, T2Example, T3Example>, IOneOf<T3Example, T2Example, T1Example>
{
}

public static class OneOfExtensions
{
    public static IOneOf<T1Example, T2Example, T3Example> ToReturn(T1Example t1)
    {
        return (Is_T1Example)t1;
    }
}

public sealed class Dependncy
{
    public async Task Handle(IOneOf<T1Example, T2Example, T3Example> oneOf)
    {
        var task = oneOf switch
        {
            Is_T1Example t => t.Value.Handle(),
            Is_T2Example t => t.Value.Handle(),
            Is_T3Example t => t.Value.Handle(),
            _ => Task.CompletedTask
        };

        await task;
    }
}