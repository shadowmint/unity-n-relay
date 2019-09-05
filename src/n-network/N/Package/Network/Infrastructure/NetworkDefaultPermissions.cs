using System.Threading.Tasks;

namespace N.Package.Network.Infrastructure
{
    public class NetworkDefaultPermissions : INetworkPermissions
    {
        public Task<bool> IsPermittedOnClient(NetworkCommand command, INetworkClient client)
        {
            return Task.FromResult(true);
        }

        public Task<bool> IsPermittedOnMaster(NetworkCommand command, INetworkMaster clientId, string fromClientId)
        {
            return Task.FromResult(true);
        }
    }
}