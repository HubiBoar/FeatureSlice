using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FeatureSlice;

public delegate T ServiceFactory<T>(IServiceProvider provider);

public interface IHandlerSetup
{
    public delegate Task<TResponse> Handle<TRequest, TResponse>(TRequest request)
        where TRequest : notnull
        where TResponse : notnull;

    public static IHandlerSetup Default { get; } = new DefaultSetup();

    public static ServiceFactory<IHandlerSetup> TryRegisterDefault(IServiceCollection services)
    {
        services.TryAddSingleton<IHandlerSetup, DefaultSetup>();
        return provider => provider.GetRequiredService<IHandlerSetup>();
    }

    public Handle<TRequest, TResponse> GetHandler<TRequest, TResponse>
    (
        IServiceProvider provider,
        Handle<TRequest, TResponse> handle
    )
        where TRequest : notnull
        where TResponse : notnull;

    internal sealed class DefaultSetup : IHandlerSetup
    {
        public Handle<TRequest, TResponse> GetHandler<TRequest, TResponse>
        (
            IServiceProvider provider,
            Handle<TRequest, TResponse> handle
        )
            where TRequest : notnull
            where TResponse : notnull
        {
            return handle;  
        }
    }
}