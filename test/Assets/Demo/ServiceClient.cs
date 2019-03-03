using System;
using System.Threading;
using System.Threading.Tasks;
using N.Package.Relay;
using N.Package.Relay.Events.Client;
using N.Package.Relay.Events.Client.In;
using N.Package.Relay.Infrastructure.TransactionManager;
using UnityEngine;

namespace WsTest
{
    class ServiceClient : IRelayClientEventHandler
    {
        private bool _active;

        private RelayTransactionManager _transactionManager;

        private RelayClient _client;
        
        public bool connected;

        public async Task Start(string remote, RelayClientOptions options)
        {
            if (_active)
            {
                await _client.Disconnect();
            }

            _active = true;
            _transactionManager = new RelayTransactionManager();
            _transactionManager.SetEventLoop(true);

            _client = new RelayClient(this, _transactionManager);
            await _client.Connect(remote, options);
        }

        public void Halt()
        {
            _client?.Disconnect();
        }

        public void OnConnected()
        {
            Trace("Connected");
            connected = true;
        }

        public void OnDisconnected()
        {
            Trace("Disconnected");
            _active = false;
            _transactionManager.SetEventLoop(false);
            connected = false;
        }

        public void OnError(Exception error)
        {
            Trace($"Error: {error}");
        }

        public void OnWarning(Exception error, string message)
        {
            Trace($"Warning: {message}: {error}");
        }

        public void OnMessage(N.Package.Relay.Events.Client.In.MessageToClient message)
        {
            Trace($"Received an incoming message: {message.data}");
        }

        public void OnMasterDisconnected(MasterDisconnected masterDisconnected)
        {
            Trace($"Master is gone: {masterDisconnected.reason}");
        }

        private void Trace(string message)
        {
            Debug.Log($"CLIENT: {message}");
        }

        public Task Send<T>(T message)
        {
            return _client.Send(message);
        }
    }
}