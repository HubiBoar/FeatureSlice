using OneOf.Types;
using OneOf;

namespace FeatureSlice;

public static partial class Messaging
{
    public delegate Task Registration();

    public interface ISetup
    {
        public delegate Task<OneOf<Success, Disabled, Error>> Receive<TMessage>(TMessage request);

        public Task<OneOf<Success, Disabled, Error>> Send<TRequest>(TRequest request, string consumerName, Receive<TRequest> receive);

        public Task<OneOf<Success, Error>> Register<TRequest>(string consumerName, Receive<TRequest> receiver);
    }
}