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

public sealed record Is<T>(T Value) : IOneOf<T, T2Example, T3Example>, IOneOf<T1Example, T, T3Example>, IOneOf<T1Example, T2Example, T>
{
    public static explicit operator Is<T>(T value)
    {
        return new Is<T>(value);
    }
}

public static class OneOfExtensions
{
    public static IOneOf<T1Example, T2Example, T3Example> ToReturn(T1Example t1)
    {
        return (Is<T1Example>)t1;
    }
}

public sealed class Dependncy
{
    public async Task Handle(IOneOf<T1Example, T2Example, T3Example> oneOf)
    {
        var task = oneOf switch
        {
            Is<T1Example> t => t.Value.Handle(),
            Is<T2Example> t => t.Value.Handle(),
            Is<T3Example> t => t.Value.Handle(),
            _ => Task.CompletedTask
        };

        await task;
    }
}