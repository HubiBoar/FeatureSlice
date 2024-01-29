using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using OneOf;
using OneOf.Else;
using OneOf.Types;

namespace FeatureSlice;

public sealed partial class FeatureSlice<TRequest> : IRegistrable
{
    public delegate Task<OneOf<Success, Error>> Dispatch(TRequest request);

    public static void Register(IServiceCollection services)
    {
        services.AddSingleton(RegisterDispatch);

        static Dispatch RegisterDispatch(IServiceProvider provider)
        {
            return request => DispatchRequest(
                request,
                provider.GetServices<IListener>().ToList(),
                provider.GetServices<IMethod<TRequest, Task<OneOf<Success, Error>>>.IPipeline>().ToList());
        }
    }

    public static Task<OneOf<Success, Error>> DispatchRequest(
        TRequest request,
        IReadOnlyCollection<IListener> listeners,
        IReadOnlyList<IMethod<TRequest, Task<OneOf<Success, Error>>>.IPipeline> pipelines)
    {
        return pipelines.RunPipeline(request, Handle);

        async Task<OneOf<Success, Error>> Handle(TRequest request)
        {
            foreach(var listener in listeners)
            {
                var result = await listener.Listen(request);

                if(result.Is(out Error error))
                {
                    return error;
                }
            }

            return new Success();
        }
    
    }

    public interface IListener
    {
        Task<OneOf<Success, Error>> Listen(TRequest request);
    }
}