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
            _commands = manager.Commands();
        }

        public NetworkEventHandler(NetworkConnectionManager manager, INetworkMaster master)
        {
            _manager = manager;
            _master = master;
            _commands = manager.Commands();
        }

        void IRelayMasterEventHandler.OnDisconnected()
        {
            _master.OnMasterConnectionLostAsync().Promise().Then(() => { }, (err) => _manager.Logger.OnError(err)).Dispatch();
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
            OnMessageAsync(message).Promise().Dispatch();
        }

        private async Task OnMessageAsync(MessageFromClient message)
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
                    try
                    {
                        var responseToClient = await _commands.ProcessIncomingMessage(_connection, networkCommand, message.data, _master, message.client_id);
                        responseToClient.commandInternalType = NetworkCommand.CommandTypeFor(responseToClient.GetType());
                        responseToClient.commandInternalIsResponse = true;
                        responseToClient.commandInternalId = networkCommand.commandInternalId;
                        await _connection.Send(responseToClient, message.client_id);
                    }
                    catch (Exception error)
                    {
                        _manager.Logger.OnError(error);
                        await _connection.Send(new NetworkCommandError()
                        {
                            message = error.Message,
                            commandInternalType = NetworkCommand.CommandTypeFor(typeof(NetworkCommandError)),
                            commandInternalIsResponse = true,
                            commandInternalId = networkCommand.commandInternalId
                        }, message.client_id);
                    }
                }
            }
            catch (Exception error)
            {
                _manager.Logger.OnError(error);
            }
        }

        public void OnClientConnected(ClientJoined message)
        {
            _master.OnMasterGotClientConnectionAsync(message).Promise().Then(() => { }, (err) => _manager.Logger.OnError(err)).Dispatch();
        }

        public void OnClientDisconnected(ClientDisconnected message)
        {
            _master.OnMasterLostClientConnectionAsync(message).Promise().Then(() => { }, (err) => _manager.Logger.OnError(err)).Dispatch();
        }

        void IRelayMasterEventHandler.OnConnected()
        {
            _master.OnMasterConnectedAsync().Promise().Then(() => { }, (err) => _manager.Logger.OnError(err)).Dispatch();
        }

        void IRelayClientEventHandler.OnError(Exception error)
        {
            _manager.Logger.OnError(error);
        }

        void IRelayClientEventHandler.OnConnected()
        {
            _client.OnConnectedToMasterAsync().Promise().Then(() => { }, (err) => _manager.Logger.OnError(err)).Dispatch();
        }

        void IRelayClientEventHandler.OnDisconnected()
        {
            _client.OnDisconnectedFromMasterAsync("Local disconnect").Promise().Then(() => { }, (err) => _manager.Logger.OnError(err)).Dispatch();
        }

        public void OnWarning(Exception error, string message)
        {
            _manager.Logger.OnError(error);
        }

        public void OnMessage(MessageToClient message)
        {
            OnMessageAsync(message).Promise().Dispatch();
        }

        private async Task OnMessageAsync(MessageToClient message)
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
                    try
                    {
                        var responseToMaster = await _commands.ProcessIncomingMessage(_connection, networkCommand, message.data, _client);
                        responseToMaster.commandInternalType = NetworkCommand.CommandTypeFor(responseToMaster.GetType());
                        responseToMaster.commandInternalIsResponse = true;
                        responseToMaster.commandInternalId = networkCommand.commandInternalId;
                        await _connection.Send(responseToMaster);
                    }
                    catch (Exception error)
                    {
                        _manager.Logger.OnError(error);
                        await _connection.Send(new NetworkCommandError()
                        {
                            message = error.Message,
                            commandInternalType = NetworkCommand.CommandTypeFor(typeof(NetworkCommandError)),
                            commandInternalIsResponse = true,
                            commandInternalId = networkCommand.commandInternalId
                        });
                    }
                }
            }
            catch (Exception error)
            {
                _manager.Logger.OnError(error);
            }
        }

        public void OnMasterDisconnected(MasterDisconnected masterDisconnected)
        {
            _client.OnDisconnectedFromMasterAsync(masterDisconnected.reason).Promise().Then(() => { }, (err) => _manager.Logger.OnError(err))
                .Dispatch();
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