using OneOf.Types;
using OneOf;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice.Static;

public interface IMessageConsumer<TInput>
{
    public abstract static Task<OneOf<Success, Error>> OnMessage(TInput input);
}
public interface IMessageDispatcher
{
    public abstract static Task Send<TInput, TSlice>(TInput input)
        where TSlice : IMessageConsumer<TInput>;
}
public class MessageDispatcher : IMessageDispatcher
{
    public static Task Send<TInput, TSlice>(TInput input) where TSlice : IMessageConsumer<TInput>
    {
        return TSlice.OnMessage(input);
    }
}

public record ExampleMessage1();
public interface IExampleConsumer1 : IMessageConsumer<ExampleMessage1>
{
}
public class ExampleConsumer1 : IExampleConsumer1
{
    public static async Task<OneOf<Success, Error>> OnMessage(ExampleMessage1 input)
    {
        return new Success();
    }
}


public interface IMethod<TInput, TOutput>
{
    public static abstract Task<TOutput> Handle(TInput input);
}
public interface IMethodDispatcher
{
    public abstract static Task<TOutput> Send<TInput, TOutput, TSlice>(TInput input)
        where TSlice : IMethod<TInput, TOutput>;
}
public class MethodDispatcher : IMethodDispatcher
{
    public static Task<TOutput> Send<TInput, TOutput, TSlice>(TInput input) where TSlice : IMethod<TInput, TOutput>
    {
        return TSlice.Handle(input);
    }
}

public record ExampleInput1();
public record ExampleOutput1();
public interface IExampleMethod1 : IMethod<ExampleInput1, ExampleOutput1>
{
}

public class ExampleMethod1 : IExampleMethod1
{
    public static Task<ExampleOutput1> Handle(ExampleInput1 input)
    {
        return Task.FromResult(new ExampleOutput1());
    }
}

public record ExampleInput2();
public record ExampleOutput2();
public interface IExampleMethod2 : IMethod<ExampleInput2, ExampleOutput2>
{
}
public class ExampleMethod2 : IExampleMethod2
{
    public static Task<ExampleOutput2> Handle(ExampleInput2 input)
    {
        return Task.FromResult(new ExampleOutput2());
    }
}


public class Example<TMethodDispatcher, TExampleMethod1, TExampleMethod2, TMessageDispatcher, TExampleConsumer1>
    where TMethodDispatcher : IMethodDispatcher
    where TExampleMethod1 : IExampleMethod1
    where TExampleMethod2 : IExampleMethod2
    where TMessageDispatcher : IMessageDispatcher
    where TExampleConsumer1 : IExampleConsumer1
{
    public Task<ExampleOutput1> Test1()
    {
        return TMethodDispatcher.Send<ExampleInput1, ExampleOutput1, TExampleMethod1>(new ExampleInput1());
    }

    public Task<ExampleOutput2> Test2()
    {
        return IMethodDispatcher.Send<ExampleInput2, ExampleOutput2, TExampleMethod2>(new ExampleInput2());
    }

    public Task TestConsumer()
    {
        return TMessageDispatcher.Send<ExampleMessage1, TExampleConsumer1>(new ExampleMessage1());
    }
}

public class Example2<TDependency>
    where TDependency : IMethodDispatcher, IExampleMethod1, IExampleMethod2, IMessageDispatcher, IExampleConsumer1
{
    public Task<ExampleOutput1> Test1()
    {
        return TDependency.Send<ExampleInput1, ExampleOutput1, TDependency>(new ExampleInput1());
    }

    public Task<ExampleOutput2> Test2()
    {
        return TDependency.Send<ExampleInput2, ExampleOutput2, TDependency>(new ExampleInput2());
    }

    public Task TestConsumer()
    {
        return TDependency.Send<ExampleMessage1, TDependency>(new ExampleMessage1());
    }
}

public class Module : IMethodDispatcher, IExampleMethod1, IExampleMethod2, IMessageDispatcher, IExampleConsumer1
{
    public void Register(IServiceCollection services)
    {
        services.AddSingleton<Example<MethodDispatcher, ExampleMethod1, ExampleMethod2, MessageDispatcher, ExampleConsumer1>>();
        services.AddSingleton<Example2<Module>>();
    }

    public static Task<TOutput> Send<TInput, TOutput, TSlice>(TInput input) where TSlice : IMethod<TInput, TOutput> => MethodDispatcher.Send<TInput, TOutput, TSlice>(input);

    public static Task Send<TInput, TSlice>(TInput input) where TSlice : IMessageConsumer<TInput> => MessageDispatcher.Send<TInput, TSlice>(input);

    static Task<ExampleOutput1> IMethod<ExampleInput1, ExampleOutput1>.Handle(ExampleInput1 input) => ExampleMethod1.Handle(input);

    static Task<ExampleOutput2> IMethod<ExampleInput2, ExampleOutput2>.Handle(ExampleInput2 input) => ExampleMethod2.Handle(input);

    static Task<OneOf<Success, Error>> IMessageConsumer<ExampleMessage1>.OnMessage(ExampleMessage1 input) => ExampleConsumer1.OnMessage(input);
}