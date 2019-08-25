using System.Threading.Tasks;

namespace N.Package.Network
{
    public abstract class NetworkCommandHandler<TRequest, TResponse> where TResponse : NetworkCommand where TRequest : NetworkCommand
    {
        public abstract Task<TResponse> ProcessRequestOnMaster(INetworkMaster master, TRequest request);
        public abstract Task<TResponse> ProcessRequestOnClient(INetworkClient client, TRequest request);
    }
}