using System;
using System.Threading.Tasks;
using N.Package.Promises;
using N.Package.Relay;
using N.Package.Relay.Events.Master;
using N.Package.Relay.Events.Master.In;
using N.Package.Relay.Infrastructure;
using N.Package.Relay.Infrastructure.TransactionManager;
using UnityEngine;

namespace WsTest
{
    class ServiceMaster : IRelayMasterEventHandler
    {
        private bool _active;

        private RelayTransactionManager _transactionManager;

        private RelayMaster _master;

        public async Task Start(string remote, RelayMasterOptions options)
        {
            if (_active)
            {
                await _master.Disconnect();
            }

            _active = true;
            _transactionManager = new RelayTransactionManager();
            _transactionManager.SetEventLoop(true);

            _master = new RelayMaster(this, _transactionManager);
            await _master.Connect(remote, options);
        }

        public void Halt()
        {
            _master?.Disconnect();
        }

        public void OnConnected()
        {
            Trace("Connected");
        }

        public void OnDisconnected()
        {
            Trace("Disconnected");
            _active = false;
            _transactionManager.SetEventLoop(false);
        }

        public void OnError(Exception error)
        {
            Trace($"Error: {error}");
        }

        public void OnWarning(RelayException relayException, string message)
        {
            Trace($"Warning: {message}: {relayException}");
        }

        public void OnMessage(MessageFromClient messageFromClient)
        {
            Trace($"Received an incoming message from {messageFromClient.client_id}: {messageFromClient.data}");

            _master.Send(messageFromClient.client_id, new DemoMessage
            {
                Message = messageFromClient.data
            }).Dispatch();
            Trace($"Echo sent!");
        }

        public void OnClientConnected(ClientJoined clientJoined)
        {
            Trace($"New client connected: {clientJoined.client_id}");
        }

        public void OnClientDisconnected(ClientDisconnected clientDisconnected)
        {
            Trace($"Client parted: {clientDisconnected.client_id}");
        }

        private void Trace(string message)
        {
            Debug.Log($"MASTER: {message}");
        }

    }
}