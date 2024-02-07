using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;

namespace FeatureSlice.FluentGenerics.Interfaces2;

public interface IConsumer<TRequest> : IMethod<TRequest, Task<OneOf<Success, Error>>>
{
}

public static class ConsumerFeatureSlice
{
    public interface Default<TRequest> : DelegateFeatureSlice.Default<TRequest, Success>
    {
        protected static void RegisterBase<TDispatch, TConsumer>(IServiceCollection services, DispatchConverter<TDispatch> converter)
            where TConsumer : class, IConsumer<TRequest>
            where TDispatch : Delegate
        {
            services.AddSingleton<TConsumer>();
            RegisterBase(services, null, converter);
        }
    }

    public interface Flag<TRequest> : DelegateFeatureSlice.Flag<TRequest, Success>
    {
        protected static void RegisterBase<TFlag, TDispatch, TConsumer>(IServiceCollection services, DispatchConverter<TDispatch> converter)
            where TConsumer : class, IConsumer<TRequest>
            where TDispatch : Delegate
            where TFlag : IFeatureFlag
        {
            services.AddSingleton<TConsumer>();
            RegisterBase<TFlag, TDispatch>(services, null, converter);
        }
    }
}