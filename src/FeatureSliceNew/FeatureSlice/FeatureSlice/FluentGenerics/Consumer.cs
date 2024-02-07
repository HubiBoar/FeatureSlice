using OneOf;
using OneOf.Types;

namespace FeatureSlice.FluentGenerics;

public interface IConsumer<TRequest> : IMethod<TRequest, Task<OneOf<Success, Error>>>
{
}