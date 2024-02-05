using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;
using FeatureSlice;
using Microsoft.AspNetCore.Builder;

namespace FeatureSliceBuilderApproach;

public struct Disabled;

public delegate Task<OneOf<TResponse, Error>> Dispatch<TRequest, TResponse>(IServiceProvider provider, TRequest request);

public interface IHandler<TRequest, TResponse>
{
    public Task<OneOf<TResponse, Error>> Handle(TRequest request);
}

public interface IArguments
{
    public IServiceCollection Services { get; }
}

public class Arguments : IArguments
{
    public IServiceCollection Services { get; }

    public Arguments(IServiceCollection services)
    {
        Services = services;
    }
}

public interface IFeatureSliceBuilder
{
}

public interface IFeatureSliceBuilder<TSelf> : IFeatureSliceBuilder
    where TSelf : IFeatureSliceBuilder<TSelf>
{
}

public static class SliceBuilderExtensions
{
    public static void Endoint<TEndpoint>(this IFeatureSliceBuilder featureSlice, HostExtender<WebApplication> hostExtender)
        where TEndpoint : Feature.IEndpoint
    {
        hostExtender.Map<TEndpoint>();
    }
}

public sealed class FeatureSliceOptions<TRequest, TResponse> : IFeatureSliceBuilder<FeatureSliceOptions<TRequest, TResponse>>
{
    public Dispatch<TRequest, TResponse> Handler<THandler>(IServiceCollection services)
        where THandler : class, IHandler<TRequest, TResponse>
    {
        services.AddSingleton<THandler>();

        return (provider, request) => provider.GetRequiredService<THandler>().Handle(request);
    }
}



public interface IFeatureSlice<TRequest, TResponse> : IFeatureSlice<TRequest, TResponse, Arguments>
{
}

public interface IFeatureSlice<TRequest, TResponse, TArgs>
    where TArgs : IArguments
{
    public static abstract Dispatch<TRequest, TResponse> Build(FeatureSliceOptions<TRequest, TResponse> builder, TArgs args);

    public interface IRegistrable<TSelf, TDispatch> : IFeatureSlice<TRequest, TResponse, TArgs>
        where TSelf : class, IRegistrable<TSelf, TDispatch>
        where TDispatch : Delegate
    {
        public static void RegisterInternal(TArgs args)
        {
            var dispatch = TSelf.Build(new FeatureSliceOptions<TRequest, TResponse>(), args);
            args.Services.AddSingleton<TDispatch>(provider => TSelf.Convert(provider, dispatch));
        }

        public abstract static void Register(TArgs args);

        public static abstract TDispatch Convert(IServiceProvider provider, Dispatch<TRequest, TResponse> dispatch);
    }
}

public partial class Example : IFeatureSlice<Example.Request, Example.Response, Example.Arguments>
{
    public record Arguments(IServiceCollection Services, HostExtender<WebApplication> HostExtender) : IArguments;

    public record Request();
    public record Response();

    public static Dispatch<Request, Response> Build(FeatureSliceOptions<Request, Response> options, Arguments arguments)
    {
        options.Endoint<Endpoint>(arguments.HostExtender);

        return options.Handler<Handler>(arguments.Services);
    }

    private class Handler : IHandler<Request, Response>
    {
        public Task<OneOf<Response, Error>> Handle(Request request)
        {
            throw new NotImplementedException();
        }
    }

    private class Endpoint : Feature.IEndpoint
    {
        public static Feature.EndpointSetup Setup => throw new NotImplementedException();
    }
}

public class ExampleUsage
{
    public static void Use(Example.Dispatch dispatch)
    {
        dispatch(new Example.Request());
    }

    public static void Register(IServiceCollection services, HostExtender<WebApplication> hostExtender)
    {
        Example.Register(new Example.Arguments(services, hostExtender));
    }
}


//Auto Generated
public partial class Example : IFeatureSlice<Example.Request, Example.Response, Example.Arguments>
    .IRegistrable<Example, Example.Dispatch>
{
    public delegate Task<OneOf<Response, Error>> Dispatch(Request request);

    public static Dispatch Convert(IServiceProvider provider, Dispatch<Request, Response> dispatch)
    {
        return (request) => dispatch(provider, request); 
    }

    public static void Register(Arguments arguments)
    {
        IFeatureSlice<Request, Response, Arguments>.IRegistrable<Example, Dispatch>.RegisterInternal(arguments);
    }

}