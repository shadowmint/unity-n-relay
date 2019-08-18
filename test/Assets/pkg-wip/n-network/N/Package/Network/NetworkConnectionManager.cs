using System.Threading.Tasks;
using N.Package.Network.Infrastructure;

namespace N.Package.Network
{
    public abstract class NetworkConnectionManager
    {
        /// <summary>
        /// Return the remote end point URL to connect through with the relay.
        /// </summary>
        protected abstract Task<string> Remote { get; }

        /// <summary>
        /// Implement this to do common setup for any network manager instance.
        /// </summary>
        protected abstract Task Configure();

        /// <summary>
        /// Register a command handler to handle networking events.
        /// </summary>
        protected void Register(NetworkCommandType fromClient, NetworkCommandHandler p1)
        {
            throw new System.NotImplementedException();
        }
    }
}