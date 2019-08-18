using System;
using System.Threading.Tasks;
using N.Package.Promises;
using N.Package.Relay;
using N.Package.Relay.Events.Client.In;
using N.Package.Relay.Infrastructure.TransactionManager;
using N.Package.Relay.Standard;
using Titan.Client.Infrastructure;
using Titan.Client.WorldState;
using Titan.Core.Events;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Titan.Client.NetworkEvents
{
    /// <summary>
    /// We may for debugging purposes have multiple clients for a single runtime!
    /// </summary>
    public class StandardRelayClient : IRelayClientEventHandler, IClientConnection, IStandardRelay
    {
        private readonly RelayClientOptions _options;

        private readonly ClientEventHandler _eventHandler;

        private readonly RelayTransactionManager _transactionManager;

        private bool _connected;

        private RelayClient _service;

        private Action<StandardRelayClient> _onConnected;

        private readonly Action<StandardRelayClient> _onDisconnect;

        private WorldClient _client;

        public string ClientName { get; private set; }

        public static StandardRelayClient Connect(string remote, RelayClientOptions options, Action<StandardRelayClient> onConnected,
            Action<StandardRelayClient> onDisconnect, Action<Exception> onError = null)
        {
            var instance = new StandardRelayClient(options, new ClientEventHandler(onError), onDisconnect);
            instance.Connect(remote, onConnected);
            return instance;
        }

        private StandardRelayClient(RelayClientOptions options, ClientEventHandler eventHandler, Action<StandardRelayClient> onDisconnect)
        {
            _options = options;
            _eventHandler = eventHandler;
            _onDisconnect = onDisconnect;
            _transactionManager = new RelayTransactionManager();
            N.Package.GameSystems.Systems.Registry.Bind(this);
        }

        private void Connect(string remote, Action<StandardRelayClient> onConnected)
        {
            if (_connected)
            {
                onConnected(this);
                return;
            }

            _onConnected = onConnected;
            _service = new RelayClient(this, _transactionManager);
            _transactionManager.SetEventLoop(true);
            _service.Connect(remote, _options)
                .Promise()
                .Then(() =>
                {
                    _connected = true;
                    ClientName = _options.metadata.name;
                }, OnError)
                .Dispatch();
        }

        public void Halt()
        {
            if (!_connected) return;
            _transactionManager.SetEventLoop(false);
            _service.Disconnect().Dispatch();
            _service = null;
            Object.Destroy(_client.ui);
            _client.ui = null;
            _client.connection = null;
            _onDisconnect?.Invoke(this);
        }

        public Task Send<T>(T message) where T : IClientMessage
        {
            return _service.Send(message);
        }

        public WorldClient GetClient()
        {
            return _client;
        }

        public bool IsActive => true;
        
        public bool IsDebug { get; set; }

        public void OnDisconnected()
        {
            Halt();
        }

        public void OnMessage(MessageToClient message)
        {
            _eventHandler.MasterMessage(message);
        }

        public void OnMasterDisconnected(MasterDisconnected message)
        {
            _eventHandler.MasterParted(message);
        }

        public void OnError(Exception error)
        {
            Debug.LogException(error);
            _eventHandler.OnError(error);
        }

        public void OnWarning(Exception error, string message)
        {
            Debug.LogWarning($"{message}: {error}");
        }

        public void OnConnected()
        {
            _onConnected?.Invoke(this);
            _onConnected = null;
        }

        public void BindClient(WorldClient client)
        {
            _eventHandler.BindClient(client);
            _client = client;
        }

        public string GetClientIdentity()
        {
            return _options.metadata.name;
        }
    }
}