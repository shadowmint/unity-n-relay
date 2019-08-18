using System;
using N.Package.Relay;
using N.Package.Relay.Events.Client.In;
using N.Package.Relay.Events.Master.In;
using N.Package.Relay.Infrastructure;

namespace N.Package.Network.Infrastructure
{
    public class NetworkEventHandler : IRelayClientEventHandler, IRelayMasterEventHandler
    {
        private readonly NetworkConnectionManager _manager;
        private readonly INetworkClient _client;
        private readonly INetworkMaster _master;
        private NetworkCommandGroup _commands;

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
            throw new NotImplementedException();
        }

        void IRelayMasterEventHandler.OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnWarning(RelayException error, string message)
        {
            throw new NotImplementedException();
        }

        public void OnMessage(MessageFromClient message)
        {
            throw new NotImplementedException();
        }

        public void OnClientConnected(ClientJoined message)
        {
            throw new NotImplementedException();
        }

        public void OnClientDisconnected(ClientDisconnected message)
        {
            throw new NotImplementedException();
        }

        void IRelayMasterEventHandler.OnConnected()
        {
            throw new NotImplementedException();
        }

        void IRelayClientEventHandler.OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        void IRelayClientEventHandler.OnConnected()
        {
            throw new NotImplementedException();
        }

        void IRelayClientEventHandler.OnDisconnected()
        {
            throw new NotImplementedException();
        }

        public void OnWarning(Exception relayException, string message)
        {
            throw new NotImplementedException();
        }

        public void OnMessage(MessageToClient message)
        {
            throw new NotImplementedException();
        }

        public void OnMasterDisconnected(MasterDisconnected masterDisconnected)
        {
            throw new NotImplementedException();
        }
    }
}