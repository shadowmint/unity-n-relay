using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using N.Package.Network.Infrastructure;
using N.Package.Relay;
using N.Package.Relay.Infrastructure.TransactionManager;

namespace N.Package.Network
{
    public abstract class NetworkConnectionManager : IDisposable
    {
        private readonly NetworkCommandGroup _commands = new NetworkCommandGroup();

        private readonly List<NetworkActiveService> _active = new List<NetworkActiveService>();

        private bool _initialized;

        private INetworkLogger _logger;

        private INetworkPermissions _permissions;

        /// <summary>
        /// Return the remote end point URL to connect through with the relay.
        /// </summary>
        protected abstract Task<string> Remote { get; }

        /// <summary>
        /// Return the timeout for network commands.
        /// </summary>
        public abstract Task<TimeSpan> NetworkCommandTimeout { get; }

        /// <summary>
        /// Implement this to do common setup for any network manager instance.
        /// </summary>
        protected abstract Task Configure();

        /// <summary>
        /// Return the network logging utility
        /// </summary>
        public virtual INetworkLogger Logger => _logger ?? (_logger = new NetworkDefaultLogger());

        /// <summary>
        /// Return the network permission utility
        /// </summary>
        public virtual INetworkPermissions Permissions => _permissions ?? (_permissions = new NetworkDefaultPermissions());

        /// <summary>
        /// Register a command handler to handle networking events.
        /// </summary>
        public void Register<TRequest, TResponse>(NetworkCommandType source, NetworkCommandHandler<TRequest, TResponse> handler)
            where TResponse : NetworkCommand
            where TRequest : NetworkCommand
        {
            _commands.Register(source, handler);
        }

        public void Disconnect(NetworkConnection connection)
        {
            _active.RemoveAll(i => i.NetworkConnection == connection);
            connection.Dispose();
        }

        public async Task Connect(INetworkMaster master)
        {
            await Initialize();

            var transactionManager = new RelayTransactionManager();
            var options = await master.MasterOptions;
            var remote = await Remote;
            var eventHandler = new NetworkEventHandler(this, master);
            var masterService = new RelayMaster(eventHandler, transactionManager);
            var networkConnection = new NetworkConnection(this, masterService, eventHandler);

            // Try to connect
            transactionManager.SetEventLoop(true);
            master.NetworkConnection = networkConnection;
            await masterService.Connect(remote, options);

            // Save active connections
            _active.Add(new NetworkActiveService()
            {
                NetworkConnection = networkConnection,
                TransactionManager = transactionManager
            });
        }

        public async Task Connect(INetworkClient client)
        {
            await Initialize();

            var transactionManager = new RelayTransactionManager();
            var options = await client.ClientOptions;
            var remote = await Remote;
            var eventHandler = new NetworkEventHandler(this, client);
            var clientService = new RelayClient(eventHandler, transactionManager);
            var networkConnection = new NetworkConnection(this, clientService, eventHandler);

            // Try to connect
            transactionManager.SetEventLoop(true);
            client.NetworkConnection = networkConnection;
            await clientService.Connect(remote, options);

            // Save active connections
            _active.Add(new NetworkActiveService()
            {
                NetworkConnection = networkConnection,
                TransactionManager = transactionManager
            });
        }

        public void Dispose()
        {
            _active.ToList().ForEach(i =>
            {
                i.TransactionManager.SetEventLoop(false);
                i.NetworkConnection.Dispose();
            });
        }

        private Task Initialize()
        {
            if (_initialized) return Task.CompletedTask;
            _initialized = true;
            return Configure();
        }

        public NetworkCommandGroup Commands()
        {
            return _commands;
        }
    }
}