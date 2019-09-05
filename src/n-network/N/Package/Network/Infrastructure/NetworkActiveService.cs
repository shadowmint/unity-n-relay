using N.Package.Relay.Infrastructure.TransactionManager;

namespace N.Package.Network.Infrastructure
{
    public class NetworkActiveService
    {
        public RelayTransactionManager TransactionManager { get; set; }
        public NetworkConnection NetworkConnection { get; set; }
    }
}