using System.Threading.Tasks;
using UnityEngine;

namespace N.Package.Network.Infrastructure
{
    public class NetworkCommandGroup
    {
        public NetworkConnection NetworkConnection { get; set; }

        public NetworkCommandGroup Clone()
        {
            return new NetworkCommandGroup();
        }

        public void Register<TRequest, TResponse>(NetworkCommandType source, NetworkCommandHandler<TRequest, TResponse> handler)
            where TResponse : NetworkCommand
            where TRequest : NetworkCommand
        {
        }

        public Task<NetworkCommand> ProcessIncomingMessage(NetworkCommand command, string source, INetworkMaster master)
        {
            Debug.LogWarning("Not implemented!");
            return Task.FromResult(new NetworkCommand());
        }

        public Task<NetworkCommand> ProcessIncomingMessage(NetworkCommand networkCommand, string messageData, INetworkClient client)
        {
            Debug.LogWarning("Not implemented!");
            return Task.FromResult(new NetworkCommand());
        }
    }
}