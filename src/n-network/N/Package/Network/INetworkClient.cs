using System;
using System.Threading.Tasks;
using N.Package.Relay;

namespace N.Package.Network
{
    public interface INetworkClient
    {
        NetworkConnection NetworkConnection { get; set; }

        Task<RelayClientOptions> ClientOptions { get; }

        Task OnConnectedToMasterAsync();

        Task OnDisconnectedFromMasterAsync(string masterDisconnectedReason);
    }
}