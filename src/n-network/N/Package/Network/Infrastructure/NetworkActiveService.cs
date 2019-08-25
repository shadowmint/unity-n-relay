using N.Package.Relay.Infrastructure.TransactionManager;

namespace N.Package.Network
{
    public class NetworkActiveService
    {
        public RelayTransactionManager TransactionManager { get; set; }
        public NetworkConnection NetworkConnection { get; set; }
    }
}