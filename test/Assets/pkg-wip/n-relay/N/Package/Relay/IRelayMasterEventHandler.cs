using System;
using N.Package.Relay.Events.Master;
using N.Package.Relay.Events.Master.In;
using N.Package.Relay.Infrastructure;

namespace N.Package.Relay
{
    public interface IRelayMasterEventHandler
    {
        void OnDisconnected();
        void OnError(Exception error);
        void OnWarning(RelayException error, string message);
        void OnMessage(MessageFromClient message);
        void OnClientConnected(ClientJoined message);
        void OnClientDisconnected(ClientDisconnected message);
        void OnConnected();
    }
}