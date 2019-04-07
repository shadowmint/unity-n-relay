using System;
using System.Threading.Tasks;
using N.Package.Promises;
using N.Package.Relay.Events.Client;
using N.Package.Relay.Events.Client.In;
using N.Package.Relay.Events.Client.Out;
using N.Package.Relay.Infrastructure;
using N.Package.Relay.Infrastructure.Events;
using N.Package.Relay.Infrastructure.EventStream;
using N.Package.Relay.Infrastructure.Model;
using N.Package.Relay.Infrastructure.TransactionManager;
using UnityEngine;
using MessageFromClient = N.Package.Relay.Events.Client.Out.MessageFromClient;
using MessageToClient = N.Package.Relay.Events.Client.In.MessageToClient;

namespace N.Package.Relay
{
    public class RelayClient
    {
        private readonly IRelayClientEventHandler _eventHandler;

        private readonly RelayTransactionManager _transactionManager;

        private RelayEventStream _eventStream;

        private RelayClientOptions _options;

        private readonly RelaySerializationHelper _serializer;

        private readonly RelayAuthService _auth;

        public RelayClient(IRelayClientEventHandler eventHandler, RelayTransactionManager transactionManager)
        {
            _eventHandler = eventHandler;
            _transactionManager = transactionManager;
            _serializer = new RelaySerializationHelper();
            _auth = new RelayAuthService();
        }

        /// <summary>
        /// Connect this to the remote relay service.
        /// </summary>
        public async Task Connect(string remote, RelayClientOptions options = null)
        {
            _eventStream = new RelayEventStream(OnRelayEvent);
            _options = options;
            await _eventStream.Connect(remote, options);
        }

        /// <summary>
        /// Disconnect from the remote service
        /// </summary>
        /// <returns></returns>
        public async Task Disconnect()
        {
            if (_eventStream == null) return;
            var events = _eventStream;
            _eventStream = null;
            await events.Disconnect();
        }

        /// <summary>
        /// Perform the basic initialization workflow
        /// </summary>
        /// <returns></returns>
        private async Task InitializeClient()
        {
            // Auth
            var deferredAuth = new RelayDeferredTransaction(_options.transactionTimeout);
            var deferredAuthTask = _transactionManager.WaitFor(deferredAuth);
            await _eventStream.Send(new Auth()
            {
                transaction_id = deferredAuth.TransactionId,
                request = _auth.GenerateAuthRequest(
                    deferredAuth.TransactionId,
                    _options.auth.sessionLength,
                    _options.auth.authKey,
                    _options.auth.authSecret),
            });

            // Wait for response
            try
            {
                await deferredAuthTask;
            }
            catch (Exception error)
            {
                _eventHandler.OnError(new RelayException(RelayErrorCode.InitializationFailed, error));
            }

            // Request initialization
            var deferred = new RelayDeferredTransaction(_options.transactionTimeout);
            var deferredTask = _transactionManager.WaitFor(deferred);
            await _eventStream.Send(new InitializeClient()
            {
                transaction_id = deferred.TransactionId,
                metadata = _options.metadata
            });

            // Wait for response
            try
            {
                await deferredTask;
            }
            catch (Exception error)
            {
                _eventHandler.OnError(new RelayException(RelayErrorCode.InitializationFailed, error));
                return;
            }

            // Try to join a session
            deferred = new RelayDeferredTransaction(_options.transactionTimeout);
            deferredTask = _transactionManager.WaitFor(deferred);
            await _eventStream.Send(new Join()
            {
                transaction_id = deferred.TransactionId,
                session_id = _options.sessionId
            });

            // Wait for response
            try
            {
                await deferredTask;
            }
            catch (Exception error)
            {
                _eventHandler.OnError(new RelayException(RelayErrorCode.InitializationFailed, error));
            }

            // Finally, we're connected
            _eventHandler.OnConnected();
        }

        /// <summary>
        /// Send a message to the master
        /// </summary>
        public async Task Send<T>(T data)
        {
            if (_eventStream == null)
            {
                throw new Exception("Not connected");
            }

            // Send
            var output = _serializer.Serialize(data);
            var deferred = new RelayDeferredTransaction(_options.transactionTimeout);
            var deferredTask = _transactionManager.WaitFor(deferred);
            await _eventStream.Send(new MessageFromClient()
            {
                transaction_id = deferred.TransactionId,
                data = output
            });

            // Wait for confirmation
            await deferredTask;
        }

        /// <summary>
        /// Handling incoming remote events
        /// </summary>
        private async Task OnRelayEvent(RelayEvent relayEvent, RelayEventStream eventStream)
        {
            switch (relayEvent.EventType)
            {
                case RelayEventType.Connected:
                    await InitializeClient();
                    break;
                case RelayEventType.Disconnected:
                    _eventHandler.OnDisconnected();
                    break;
                case RelayEventType.IncomingEvent:
                    DispatchEvent(relayEvent.IncomingEvent);
                    break;
                case RelayEventType.Error:
                    _eventHandler.OnError(relayEvent.Error);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DispatchEvent(RelayIncomingEvent incomingEvent)
        {
            if (incomingEvent is TransactionResult)
            {
                _transactionManager.HandleTransactionResult((TransactionResult) incomingEvent);
                return;
            }

            var clientEvent = incomingEvent as ClientIncomingEvent;
            if (clientEvent == null)
            {
                _eventHandler.OnWarning(new RelayException(RelayErrorCode.UnknownEvent), $"Ignored incoming unknown event of type {incomingEvent}");
                return;
            }

            DispatchMasterDisconnected(clientEvent as MasterDisconnected);
            DispatchClientMessage(clientEvent as MessageToClient);
        }

        private void DispatchClientMessage(MessageToClient message)
        {
            if (message == null) return;
            _eventHandler.OnMessage(message);
        }

        private void DispatchMasterDisconnected(MasterDisconnected masterDisconnected)
        {
            if (masterDisconnected == null) return;
            _eventHandler.OnMasterDisconnected(masterDisconnected);
            Disconnect().Dispatch();
        }
    }
}