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

public class FeatureSliceBuilder<TRequest, TResponse>
{
    public IServiceCollection Services { get; }

    public FeatureSliceBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public FeatureSliceBuilder<TRequest, TResponse> Endoint<TEndpoint>(HostExtender<WebApplication> hostExtender)
        where TEndpoint : Feature.IEndpoint
    {
        hostExtender.Map<TEndpoint>();

        return this;
    }

    public Dispatch<TRequest, TResponse> Handler<THandler>()
        where THandler : class, IHandler<TRequest, TResponse>
    {
        Services.AddSingleton<THandler>();

        return (provider, request) => provider.GetRequiredService<THandler>().Handle(request);
    }
}

public interface IFeatureSlice<TRequest, TResponse, TArgs>
{
    public static abstract Dispatch<TRequest, TResponse> Build(FeatureSliceBuilder<TRequest, TResponse> builder, TArgs args);

    public interface IRegistrable<TSelf, TDispatch> : IFeatureSlice<TRequest, TResponse, TArgs>
        where TSelf : class, IRegistrable<TSelf, TDispatch>
        where TDispatch : Delegate
    {
        public static void RegisterInternal(IServiceCollection services, TArgs args)
        {
            var dispatch = TSelf.Build(new FeatureSliceBuilder<TRequest, TResponse>(services), args);
            services.AddSingleton<TDispatch>(provider => TSelf.Convert(provider, dispatch));
        }

        public abstract static void Register(IServiceCollection services, TArgs args);

        public static abstract TDispatch Convert(IServiceProvider provider, Dispatch<TRequest, TResponse> dispatch);
    }
}

public partial class Example : IFeatureSlice<Example.Request, Example.Response, Example.Arguments>
{
    public record Arguments(HostExtender<WebApplication> HostExtender);

    public record Request();
    public record Response();

    public static Dispatch<Request, Response> Build(FeatureSliceBuilder<Request, Response> builder, Arguments arguments)
    {
        return builder
            .Endoint<Endpoint>(arguments.HostExtender)
            .Handler<Handler>();
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
        Example.Register(services, new Example.Arguments(hostExtender));
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

    public static void Register(IServiceCollection services, Arguments arguments)
    {
        IFeatureSlice<Request, Response, Arguments>.IRegistrable<Example, Dispatch>.RegisterInternal(services, arguments);
    }

}