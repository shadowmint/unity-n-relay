using System;
using System.Threading.Tasks;
using Demo;
using N.Package.Relay;

namespace N.Package.Network
{
    public class NetworkConnection : IDisposable
    {
        public NetworkConnection(INetworkMaster master, NetworkConnectionManager manager, RelayMaster masterService)
        {
            throw new System.NotImplementedException();
        }

        public NetworkConnection(INetworkMaster master, NetworkConnectionManager manager, RelayClient clientService)
        {
            throw new System.NotImplementedException();
        }

        public NetworkConnection(INetworkClient client, NetworkConnectionManager manager, RelayClient clientService)
        {
            throw new NotImplementedException();
        }

        public async Task<TResponse> Execute<TRequest, TResponse>(TRequest request)
            where TResponse : NetworkCommand
            where TRequest : NetworkCommand
        {
            throw new System.NotImplementedException();
        }

        public async Task<TResponse> Execute<TRequest, TResponse>(TRequest request, string clientId)
            where TResponse : NetworkCommand
            where TRequest : NetworkCommand
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
        }
    }
}