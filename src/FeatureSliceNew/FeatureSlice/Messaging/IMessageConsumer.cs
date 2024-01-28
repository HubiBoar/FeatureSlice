using OneOf.Types;
using OneOf;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureSlice;

public struct Retry();

public partial interface IMessageConsumer<TRequest> : IMethod<TRequest, Task<OneOf<Success, Retry, Error>>>
{
    public interface ISetup
    {
        public Task Send(TRequest request);

        public Task Register(Func<TRequest, Task<OneOf<Success, Retry, Error>>> handler);
    }

    public static class Runner<TSelf>
        where TSelf : class, IMessageConsumer<TRequest>
    {
        public static Task Dispatch(TRequest request, TSelf self, ISetup setup, IReadOnlyList<IPipeline> pipelines)
        {
            pipelines.RunPipeline(request, self.Handle);
        }

        public static Func<TRequest, Task> Factory(IServiceProvider provider)
        {
            return request => Dispatch(
                request,
                provider.GetRequiredService<TSelf>(),
                provider.GetServices<IPipeline>().ToList());
        }

        public static void Register<TDispatcher>(IServiceCollection services, Func<IServiceProvider, TDispatcher> factory)
            where TDispatcher : Delegate
        {
            services.AddSingleton<TSelf>();
            services.AddSingleton(factory);
        }
    }
}