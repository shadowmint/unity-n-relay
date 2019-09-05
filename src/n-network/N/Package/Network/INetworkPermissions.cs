using System.Threading.Tasks;

namespace N.Package.Network
{
    public interface INetworkPermissions
    {
        /// <summary>
        /// Executed on the client to verify a command can be performed.
        /// Verifying permissions may be deferred, so this is a task. 
        /// </summary>
        Task<bool> IsPermittedOnClient(NetworkCommand command, INetworkClient client);

        /// <summary>
        /// Executed on the master to verify a command can be performed by a specific client.
        /// Verifying permissions may be deferred, so this is a task.
        /// </summary>
        Task<bool> IsPermittedOnMaster(NetworkCommand command, INetworkMaster clientId, string fromClientId);
    }
}