using System;
using System.Threading.Tasks;
using N.Package.Promises;
using N.Package.Relay;
using N.Package.Relay.Events.Client.In;
using N.Package.Relay.Events.Master.In;
using N.Package.Relay.Infrastructure;
using UnityEngine;

namespace N.Package.Network.Infrastructure
{
    public class NetworkEventHandler : IRelayClientEventHandler, IRelayMasterEventHandler
    {
        private readonly NetworkConnectionManager _manager;
        private readonly INetworkClient _client;
        private readonly INetworkMaster _master;
        private readonly NetworkCommandGroup _commands;
        private NetworkConnection _connection;

        public NetworkEventHandler(NetworkConnectionManager manager, INetworkClient client)
        {
            _manager = manager;
            _client = client;
            _commands = manager.CommandGroupFor(client.NetworkConnection);
        }

        public NetworkEventHandler(NetworkConnectionManager manager, INetworkMaster master)
        {
            _manager = manager;
            _master = master;
            _commands = manager.CommandGroupFor(master.NetworkConnection);
        }

        void IRelayMasterEventHandler.OnDisconnected()
        {
            _master.OnMasterConnectionLostAsync().Promise().Dispatch();
        }

        void IRelayMasterEventHandler.OnError(Exception error)
        {
            _manager.Logger.OnError(error);
        }

        public void OnWarning(RelayException error, string message)
        {
            _manager.Logger.OnWarning(error, message);
        }

        public void OnMessage(MessageFromClient message)
        {
            try
            {
                var networkCommand = JsonUtility.FromJson<NetworkCommand>(message.data);
                if (networkCommand.commandInternalIsResponse)
                {
                    _connection.ResolveTransaction(networkCommand, message.data);
                }
                else
                {
                    var responseToClient = _commands.ProcessIncomingMessage(networkCommand, message.data, _master);
                    _connection.Send(responseToClient).Promise().Then(() => { }, (err) => _manager.Logger.OnError(err)).Dispatch();
                }
            }
            catch (Exception error)
            {
                _manager.Logger.OnError(error);
            }
        }

        public void OnClientConnected(ClientJoined message)
        {
            _master.OnMasterGotClientConnectionAsync(message).Promise().Dispatch();
        }

        public void OnClientDisconnected(ClientDisconnected message)
        {
            _master.OnMasterLostClientConnectionAsync(message).Promise().Dispatch();
        }

        void IRelayMasterEventHandler.OnConnected()
        {
            _master.OnMasterConnectedAsync().Promise().Dispatch();
        }

        void IRelayClientEventHandler.OnError(Exception error)
        {
            _manager.Logger.OnError(error);
        }

        void IRelayClientEventHandler.OnConnected()
        {
            _client.OnConnectedToMasterAsync().Promise().Dispatch();
        }

        void IRelayClientEventHandler.OnDisconnected()
        {
            _client.OnDisconnectedFromMasterAsync("Local disconnect").Promise().Dispatch();
        }

        public void OnWarning(Exception error, string message)
        {
            _manager.Logger.OnError(error);
        }

        public void OnMessage(MessageToClient message)
        {
            try
            {
                var networkCommand = JsonUtility.FromJson<NetworkCommand>(message.data);
                if (networkCommand.commandInternalIsResponse)
                {
                    _connection.ResolveTransaction(networkCommand, message.data);
                }
                else
                {
                    var responseToClient = _commands.ProcessIncomingMessage(networkCommand, message.data, _client);
                    _connection.Send(responseToClient).Promise().Then(() => { }, (err) => _manager.Logger.OnError(err)).Dispatch();
                }
            }
            catch (Exception error)
            {
                _manager.Logger.OnError(error);
            }
        }

        public void OnMasterDisconnected(MasterDisconnected masterDisconnected)
        {
            _client.OnDisconnectedFromMasterAsync(masterDisconnected.reason).Promise().Dispatch();
        }

        /// <summary>
        /// Mark this event handler as specifically bound to a network connection
        /// </summary>
        public void Connected(NetworkConnection networkConnection)
        {
            _connection = networkConnection;
        }
    }
}