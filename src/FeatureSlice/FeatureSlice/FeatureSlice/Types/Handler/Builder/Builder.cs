using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

public static partial class FeatureSlice
{
    public static class WithHandler<TRequest, TResponse, THandler>
        where THandler : class, IHandler<TRequest, TResponse>
    {
        public abstract class Build<TSelf> : HandlerFeatureSlice.Default<TRequest, TResponse, THandler>
            where TSelf : Build<TSelf>
        {
            public static void Register(IServiceCollection services)
            {
                RegisterBase(services);
            }
        }
    }
}