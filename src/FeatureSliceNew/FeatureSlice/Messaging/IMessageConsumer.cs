using OneOf.Types;
using OneOf;

namespace FeatureSlice;

public struct Retry();

public interface IMessageConsumer<TRequest> : IMethod<TRequest, Task<OneOf<Success, Retry, Error>>>
{
}