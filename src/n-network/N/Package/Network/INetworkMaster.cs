using System;
using System.Threading.Tasks;
using N.Package.Relay;
using N.Package.Relay.Events.Master.In;

namespace N.Package.Network
{
    public interface INetworkMaster
    {
        Guid Identity { get; set; }
        
        NetworkConnection NetworkConnection { get; set; }

        Task<RelayMasterOptions> MasterOptions { get; }

        Task OnMasterConnectedAsync();

        Task OnMasterConnectionLostAsync();

        Task OnMasterLostClientConnectionAsync(ClientDisconnected message);

        Task OnMasterGotClientConnectionAsync(ClientJoined message);
    }
}