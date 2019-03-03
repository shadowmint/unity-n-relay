using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using N.Package.Relay.Config;
using N.Package.Relay.Events;
using N.Package.Relay.Events.Client;
using N.Package.Relay.Events.Master;
using N.Package.Relay.Events.Master.In;
using N.Package.Relay.Events.Master.Out;
using N.Package.Relay.Infrastructure;
using N.Package.Relay.Infrastructure.Events;
using N.Package.Relay.Infrastructure.EventStream;
using N.Package.Relay.Infrastructure.Model;
using N.Package.Relay.Infrastructure.TransactionManager;
using MessageFromClient = N.Package.Relay.Events.Client.Out.MessageFromClient;
using MessageToClient = N.Package.Relay.Events.Master.Out.MessageToClient;

namespace N.Package.Relay
{
    public class RelayMaster
    {
        private readonly IRelayMasterEventHandler _eventHandler;

        private readonly RelayTransactionManager _transactionManager;

        private RelayEventStream _eventStream;

        private readonly List<string> _clients = new List<string>();

        private RelayMasterOptions _options;

        private readonly RelaySerializationHelper _serializer;

        /// <summary>
        /// The set of currently connected clients
        /// </summary>
        public IEnumerable<string> Clients => _clients;

        public RelayMaster(IRelayMasterEventHandler eventHandler, RelayTransactionManager transactionManager)
        {
            _eventHandler = eventHandler;
            _transactionManager = transactionManager;
            _serializer = new RelaySerializationHelper();
        }

        /// <summary>
        /// Connect this to the remote relay service.
        /// </summary>
        public async Task Connect(string remote, RelayMasterOptions options = null)
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
        private async Task InitializeMaster()
        {
            // Request initialization
            var deferred = new RelayDeferredTransaction(_options.TransactionTimeout);
            await _eventStream.Send(new InitializeMaster()
            {
                transaction_id = deferred.TransactionId,
                metadata = _options.Metadata
            });

            // Wait for response
            try
            {
                await _transactionManager.WaitFor(deferred);
            }
            catch (Exception error)
            {
                _eventHandler.OnError(new RelayException(RelayErrorCode.InitializationFailed, error));
            }
        }

        /// <summary>
        /// Send a message to a specific client
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task Send<T>(string clientId, T data)
        {
            if (_eventStream == null)
            {
                throw new Exception("Not connected");
            }

            // Send
            var output = _serializer.Serialize(data);
            var deferred = new RelayDeferredTransaction(_options.TransactionTimeout);
            await _eventStream.Send(new MessageToClient()
            {
                transaction_id = deferred.TransactionId,
                client_id = clientId,
                data = output
            });

            // Wait for confirmation
            await _transactionManager.WaitFor(deferred);
        }

        /// <summary>
        /// Handling incoming remote events
        /// </summary>
        private async Task OnRelayEvent(RelayEvent relayEvent, RelayEventStream eventStream)
        {
            switch (relayEvent.EventType)
            {
                case RelayEventType.Connected:
                    await InitializeMaster();
                    _eventHandler.OnConnected();
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

            var masterEvent = incomingEvent as MasterIncomingEvent;
            if (masterEvent == null)
            {
                _eventHandler.OnWarning(new RelayException(RelayErrorCode.UnknownEvent), $"Ignored incoming unknown event of type {incomingEvent}");
                return;
            }

            DispatchClientDisconnected(masterEvent as ClientDisconnected);
            DispatchClientConnected(masterEvent as ClientJoined);
            DispatchClientMessage(masterEvent as N.Package.Relay.Events.Master.In.MessageFromClient);
        }

        private void DispatchClientMessage(Events.Master.In.MessageFromClient messageFromClient)
        {
            if (messageFromClient == null) return;
            _eventHandler.OnMessage(messageFromClient);
        }

        private void DispatchClientConnected(ClientJoined clientJoined)
        {
            if (clientJoined == null) return;
            _clients.Add(clientJoined.client_id);
            _eventHandler.OnClientConnected(clientJoined);
        }

        private void DispatchClientDisconnected(ClientDisconnected clientDisconnected)
        {
            if (clientDisconnected == null) return;
            _clients.Remove(clientDisconnected.client_id);
            _eventHandler.OnClientDisconnected(clientDisconnected);
        }
    }
}