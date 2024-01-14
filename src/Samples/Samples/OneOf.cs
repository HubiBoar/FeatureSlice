using OneOfGen;

namespace OneOfExample;

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
public record T3Example() : IOneOfElement
{
    public Task Handle()
    {
        return Task.CompletedTask;
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