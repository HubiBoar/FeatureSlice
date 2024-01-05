using FeatureSlice.New.Generation;

namespace ServicesExtensions;

public static class Extensions
{
    public static IExampleMethod.Response Send<T>(this IDispatcher<T> dispatcher, IExampleMethod.Request request)
        where T : IMethod<IExampleMethod.Request, IExampleMethod.Response>
    {
        return IMethod<IExampleMethod.Request, IExampleMethod.Response>.Send(dispatcher, request);
    }
}