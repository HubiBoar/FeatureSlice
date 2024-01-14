namespace OneOf2Example;

public partial record T1Example()
{
    public Task Handle()
    {
        return Task.CompletedTask;
    }
}

//Auto Generated
public partial record T1Example : IOneOf<T1Example, T2Example, T3Example>
{

}

public partial record T2Example()
{
    public Task Handle()
    {
        return Task.CompletedTask;
    }
}

//Auto Generated
public partial record T2Example : IOneOf<T1Example, T2Example, T3Example>
{
}

public partial record T3Example()
{
    public Task Handle()
    {
        return Task.CompletedTask;
    }
}

//Auto Generated
public partial record T3Example : IOneOf<T1Example, T2Example, T3Example>
{
}

public interface IOneOf<T1, T2, T3>
{
}

public static class OneOfExtensions
{
    public static IOneOf<T1Example, T2Example, T3Example> ToReturn(T1Example t1)
    {
        return t1;
    }
}

public sealed class Dependncy
{
    public async Task Handle(IOneOf<T1Example, T2Example, T3Example> oneOf)
    {
        var task = oneOf switch
        {
            T1Example t => t.Handle(),
            T2Example t => t.Handle(),
            T3Example t => t.Handle(),
            _ => Task.CompletedTask
        };

        await task;
    }
}