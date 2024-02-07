using Microsoft.Extensions.DependencyInjection;
using OneOf;
using OneOf.Types;

namespace FeatureSlice.FluentGenerics.Interfaces2;

public interface IConsumer<TRequest> : IMethod<TRequest, Task<OneOf<Success, Error>>>
{
}

public static class ConsumerFeatureSlice
{
    public static class Default<TRequest>
    {
        public static void Register<TDispatch, TConsumer>(IServiceCollection services, DelegateFeatureSlice.Default<TRequest, Success>.DispatchConverter<TDispatch> converter)
            where TConsumer : class, IConsumer<TRequest>
            where TDispatch : Delegate
        {
            services.AddSingleton<TConsumer>();
            DelegateFeatureSlice.Default<TRequest, Success>.Register(services, null, converter);
        }
    }

    public static class Flag<TRequest>
    {
        public static void Register<TFlag, TDispatch, TConsumer>(IServiceCollection services, DelegateFeatureSlice.Flag<TRequest, Success>.DispatchConverter<TDispatch> converter)
            where TConsumer : class, IConsumer<TRequest>
            where TDispatch : Delegate
            where TFlag : IFeatureFlag
        {
            services.AddSingleton<TConsumer>();
            DelegateFeatureSlice.Flag<TRequest, Success>.Register<TFlag, TDispatch>(services, null, converter);
        }
    }
}