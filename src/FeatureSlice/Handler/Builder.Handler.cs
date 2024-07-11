using Microsoft.Extensions.DependencyInjection;
using Definit.Dependencies;

namespace FeatureSlice;

public static partial class FeatureSlice<TRequest, TResponse, TDependencies>
    where TDependencies : class, IFromServices<TDependencies>
    where TRequest : notnull
    where TResponse : notnull
{
    public abstract class Build<TSelf> : HandlerBase<TSelf, TRequest, TResponse, TDependencies>
        where TSelf : Build<TSelf>, new()
    {
        public static void Register(IServiceCollection services)
        {
            var self = new TSelf();
            var handle = self.OnRequest;
            var serviceLifetime = self.ServiceLifetime;

            services
                .FeatureSlice()
                .WithHandler<Handle, TRequest, TResponse, TDependencies>(
                    (request, dep) => handle(request, dep),
                    h => h.Invoke,
                    serviceLifetime);
        }
    }
}