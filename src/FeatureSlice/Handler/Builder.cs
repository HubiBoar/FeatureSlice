using Microsoft.Extensions.DependencyInjection;
using Definit.Dependencies;
using Definit.Results;

namespace FeatureSlice;

public static partial class FeatureSlice<TRequest, TResponse, TDependencies>
    where TDependencies : class, IFromServices<TDependencies>
    where TRequest : notnull
    where TResponse : notnull
{
    public abstract class Build<TSelf> : HandlerBase<TSelf, TRequest, Result<TResponse>, TResponse, TDependencies>
        where TSelf : Build<TSelf>, new()
    {
        public static void Register(IServiceCollection services)
        {
            RegisterHandler(services);
        }
 
        public static void Register
        (
            IServiceCollection services,
            ServiceFactory<IHandlerSetup> setupFactory
        )
        {
            RegisterHandler(services, setupFactory);
        }
    }
}

public static partial class FeatureSlice<TRequest, TDependencies>
    where TDependencies : class, IFromServices<TDependencies>
    where TRequest : notnull
{
    public abstract class Build<TSelf> : HandlerBase<TSelf, TRequest, Result, Success, TDependencies>
        where TSelf : Build<TSelf>, new()
    {
        public static void Register(IServiceCollection services)
        {
            RegisterHandler(services);
        }
 
        public static void Register
        (
            IServiceCollection services,
            ServiceFactory<IHandlerSetup> setupFactory
        )
        {
            RegisterHandler(services, setupFactory);
        }
    }
}