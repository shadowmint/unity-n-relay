using System;
using System.Threading.Tasks;
using N.Package.Promises;
using N.Package.Relay;
using N.Package.Relay.Events.Master.In;
using N.Package.Relay.Infrastructure;
using N.Package.Relay.Infrastructure.TransactionManager;
using N.Package.Relay.Standard;
using Titan.Core.Events;
using UnityEngine;

namespace Titan.Master.NetworkEvents
{
    public class StandardRelayMaster : IRelayMasterEventHandler
    {
        private readonly RelayMasterOptions _options;

        private readonly IStandardRelayEvents _eventHandler;

        private readonly Action<StandardRelayMaster> _onDisconnect;
        private Action<Exception> _onError;

        private readonly RelayTransactionManager _transactionManager;

        private bool _connected;

        private RelayMaster _service;

        private Action _onConnected;

        public static StandardRelayMaster Connect(string remote, IStandardRelayEvents events, RelayMasterOptions options, Action onConnected,
            Action<StandardRelayMaster> onDisconnect, Action<Exception> onError)
        {
            var instance = new StandardRelayMaster(options, events, onDisconnect, onError);
            instance.Connect(remote, onConnected);
            return instance;
        }

        private StandardRelayMaster(RelayMasterOptions options, IStandardRelayEvents eventHandler, Action<StandardRelayMaster> onDisconnect,
            Action<Exception> onError)
        {
            _options = options;
            _eventHandler = eventHandler;
            _onDisconnect = onDisconnect;
            _onError = onError;
            _transactionManager = new RelayTransactionManager();
            N.Package.GameSystems.Systems.Registry.Bind(this);
        }

        private void Connect(string remote, Action onConnected)
        {
            if (_connected)
            {
                onConnected();
                return;
            }

            _onConnected = onConnected;
            _service = new RelayMaster(this, _transactionManager);
            _transactionManager.SetEventLoop(true);
            _service.Connect(remote, _options)
                .Promise()
                .Then(() =>
                {
                    _connected = true;
                    _onError = (err) => _eventHandler.FailureDuringNormalOperation(err);
                }, OnError)
                .Dispatch();
        }


        public Task Send<T>(string clientId, T message) where T : IMasterMessage
        {
            return _service.Send(clientId, message);
        }

        public void Halt()
        {
            if (!_connected) return;
            _transactionManager.SetEventLoop(false);
            _service.Disconnect().Dispatch();
            _service = null;
            _onDisconnect?.Invoke(this);
        }

        public void OnDisconnected()
        {
            Halt();
        }

        public void OnError(Exception error)
        {
            Debug.LogException(error);
            _onError?.Invoke(error);
        }

        public void OnWarning(RelayException error, string message)
        {
            Debug.LogWarning($"{message}: {error}");
        }

        public void OnMessage(MessageFromClient messageFromClient)
        {
            _eventHandler.PlayerMessage(messageFromClient);
        }

        public void OnClientConnected(ClientJoined clientJoined)
        {
            _eventHandler.PlayerJoin(clientJoined);
        }

        public void OnClientDisconnected(ClientDisconnected clientDisconnected)
        {
            _eventHandler.PlayerParted(clientDisconnected);
        }

        public void OnConnected()
        {
            _onConnected?.Invoke();
            _onConnected = null;
        }
    }
}