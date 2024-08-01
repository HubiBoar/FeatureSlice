using Microsoft.Extensions.DependencyInjection;
using Definit.Results;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace FeatureSlice;

public interface IFeatureSlice
{
}

public interface IFromException<TResult, TResponse>
    where TResult : Result_Base<TResponse>
    where TResponse : notnull
{
    public abstract static TResult FromException(Exception exception);
}

public interface IFeatureSliceSetup<TRequest, TResult, TResponse> : IFromException<TResult, TResponse>
    where TRequest : notnull
    where TResult : Result_Base<TResponse>
    where TResponse : notnull
{
    public ServiceLifetime ServiceLifetime { get; set; }

    public DisptacherFactory<TRequest, TResult, TResponse> DispatchFactory { get; set; }


    public Handle<TRequest, TResult, TResponse> GetHandle(IServiceProvider provider);

    public Handle<TRequest, TResult, TResponse> GetDispatch(IServiceProvider provider);

    public void Extend(Action<IServiceCollection> extension);

    internal void RunExtensions(IServiceCollection services);
}

public abstract partial record FeatureSliceBase<TRequest, TResult, TResponse, TFromException>
(
    IFeatureSliceSetup<TRequest, TResult, TResponse> Options
)
: IFeatureSlice

where TRequest : notnull
where TResult : Result_Base<TResponse>
where TResponse : notnull
where TFromException : IFromException<TResult, TResponse>

{
    public Handle<TRequest, TResult, TResponse> Dispatch { get; init; } = null!;

    public static void Register<T>(IServiceCollection services)
        where T : FeatureSliceBase<TRequest, TResult, TResponse, TFromException>, new()
    {
        services.TryAdd(IDispatcher.RegisterDefault());

        var options = new T().Options;
        options.RunExtensions(services);

        services.Add
        (
            ServiceDescriptor.Describe
            (
                typeof(T),
                provider =>
                {
                    var slice = new T()
                    {
                        Dispatch = options.GetDispatch(provider)
                    };

                    return slice;
                },
                options.ServiceLifetime
            )
        );
    }

    public sealed class Setup : IFeatureSliceSetup<TRequest, TResult, TResponse>
    {
        public ServiceLifetime ServiceLifetime { get; set; }

        private readonly Func<IServiceProvider, Handle<TRequest, TResult, TResponse>> _handle;

        public Setup
        (
            Func<IServiceProvider, Handle<TRequest, TResult, TResponse>> handle,
            ServiceLifetime serviceLifetime = ServiceLifetime.Transient
        )
        {
            _handle = handle;
            ServiceLifetime = serviceLifetime;
        }

        public DisptacherFactory<TRequest, TResult, TResponse> DispatchFactory { get; set; } = DefaultDispatcher;

        private readonly List<Action<IServiceCollection>> _extensions = [];

        public Handle<TRequest, TResult, TResponse> GetHandle(IServiceProvider provider)
        {
            return async request =>
            {
                try                {
                    return await _handle(provider)(request);
                }
                catch (Exception exception)
                {
                    return TFromException.FromException(exception);
                }
            };
        }

        public Handle<TRequest, TResult, TResponse> GetDispatch(IServiceProvider provider)
        {
            var handle = GetHandle(provider);
            return DispatchFactory(provider, handle);
        }

        public void Extend(Action<IServiceCollection> extension)
        {
            _extensions.Add(extension);
        }

        private static Handle<TRequest, TResult, TResponse> DefaultDispatcher(IServiceProvider provider, Handle<TRequest, TResult, TResponse> handle)
        {
            return provider.GetRequiredService<IDispatcher>().GetDispatcher(provider, handle);
        }

        void IFeatureSliceSetup<TRequest, TResult, TResponse>.RunExtensions(IServiceCollection services)
        {
            foreach(var extension in _extensions)
            {
                extension(services);
            }
        }

        public static TResult FromException(Exception exception)
        {
            return TFromException.FromException(exception);
        }
    }
}

public static class FeatureSliceExtensions
{
    //TODO Reflection based version, try to source generate it
    public static void AddFeatureSlice<T>(this IServiceCollection services)
        where T : class, IFeatureSlice, new()
    {
        var type = typeof(T);

        var method = type.GetMethod("Register", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)!;
        var genericMethod = method.MakeGenericMethod(typeof(T));

        genericMethod.Invoke(null, [services]);
    }
}