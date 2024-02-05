using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using OneOf;
using OneOf.Types;

namespace FeatureSlice;

public struct Disabled;

public partial interface IFeatureSlice
{
    public partial interface IHandler<TRequest, TResponse> : IMethod<TRequest, Task<OneOf<TResponse, Error>>>
    { 
        public delegate Task<OneOf<TResponse, Error>> Dispatch(TRequest request);

        public interface IRegistrable<TSelf, TDispatcher> : IHandler<TRequest, TResponse>, IRegistrable<TSelf, Dispatch, TDispatcher>, IRegistrable
            where TSelf : class, IRegistrable<TSelf, TDispatcher>
            where TDispatcher : Delegate
        {
            static void IRegistrable.Register(IServiceCollection services)
            {
                Register(services, provider => request => InMemoryDispatcher.Dispatch<TRequest, TResponse, TSelf>(request, provider));
            }

            static void IRegistrable<TSelf, Dispatch, TDispatcher>.OnRegistration(
                IServiceCollection services,
                Func<IServiceProvider, Dispatch> dispatch)
            {
                services.AddFeatureManagement();
                services.AddSingleton<Publisher<TRequest>.Listen>(RegisterListener);
                Publisher<TRequest>.Listen RegisterListener(IServiceProvider provider)
                {
                    return async request => {
                        var result = await dispatch(provider)(request);
                        return result.Match<OneOf<Success, Error>>(success => new Success(), errror => errror);
                    };
                }
            }
        }

        public interface WithToggle : IMethod<TRequest, Task<OneOf<TResponse, Error>>>
        {   
            public abstract static string Name { get; }

            public delegate Task<OneOf<TResponse, Disabled, Error>> Dispatch(TRequest request);

            public interface IRegistrable<TSelf, TDispatcher> : IHandler<TRequest, TResponse>.WithToggle, IRegistrable<TSelf, Dispatch, TDispatcher>, IRegistrable
                where TSelf : class, IRegistrable<TSelf, TDispatcher>
                where TDispatcher : Delegate
            {
                static void IRegistrable.Register(IServiceCollection services)
                {
                    Register(services, provider => request => InMemoryDispatcher.Dispatch<TRequest, TResponse, TSelf>(request, TSelf.Name, provider));
                }

                static void IRegistrable<TSelf, Dispatch, TDispatcher>.OnRegistration(
                    IServiceCollection services,
                    Func<IServiceProvider, Dispatch> dispatch)
                {
                    services.AddFeatureManagement();
                    services.AddSingleton<Publisher<TRequest>.Listen>(RegisterListener);
                    Publisher<TRequest>.Listen RegisterListener(IServiceProvider provider)
                    {
                        return async request => {
                            var result = await dispatch(provider)(request);
                            return result.Match<OneOf<Success, Error>>(success => new Success(), disabled => new Success(), errror => errror);
                        };
                    }
                }
            }
        }
    }
}

public static class InMemoryDispatcher
{
    public static Task<OneOf<TResponse, Disabled, Error>> Dispatch<TRequest, TResponse, THandler>(
        TRequest request,
        string name,
        IServiceProvider provider)
        where THandler : IMethod<TRequest, Task<OneOf<TResponse, Error>>>
    {
        return Dispatch(
            request,
            provider.GetRequiredService<THandler>(),
            name,
            provider.GetRequiredService<IFeatureManager>(),
            provider.GetServices<IMethod<TRequest, Task<OneOf<TResponse, Error>>>.IPipeline>().ToList());
    }

    public static async Task<OneOf<TResponse, Disabled, Error>> Dispatch<TRequest, TResponse, THandler>(
        TRequest request,
        THandler self,
        string name,
        IFeatureManager featureManager,
        IReadOnlyList<IMethod<TRequest, Task<OneOf<TResponse, Error>>>.IPipeline> pipelines)
        where THandler : IMethod<TRequest, Task<OneOf<TResponse, Error>>>
    {
        var isEnabled = await featureManager.IsEnabledAsync(name);
        if(isEnabled == false)
        {
            return new Disabled();
        }

        var result = await Dispatch<TRequest, TResponse, THandler>(request, self, pipelines);
        return result.Match<OneOf<TResponse, Disabled, Error>>(success => success, error => error);
    }

    public static Task<OneOf<TResponse, Error>> Dispatch<TRequest, TResponse, THandler>(
        TRequest request,
        IServiceProvider provider)
        where THandler : IMethod<TRequest, Task<OneOf<TResponse, Error>>>
    {
        return Dispatch(
            request,
            provider.GetRequiredService<THandler>(),
            provider.GetServices<IMethod<TRequest, Task<OneOf<TResponse, Error>>>.IPipeline>().ToList());
    }

    public static async Task<OneOf<TResponse, Error>> Dispatch<TRequest, TResponse, THandler>(
        TRequest request,
        THandler self,
        IReadOnlyList<IMethod<TRequest, Task<OneOf<TResponse, Error>>>.IPipeline> pipelines)
        where THandler : IMethod<TRequest, Task<OneOf<TResponse, Error>>>
    {
        return await pipelines.RunPipeline(request, self.Handle);
    }
}