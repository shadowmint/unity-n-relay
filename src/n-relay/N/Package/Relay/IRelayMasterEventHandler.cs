using System;
using N.Package.Relay.Events.Master;
using N.Package.Relay.Events.Master.In;
using N.Package.Relay.Infrastructure;

namespace N.Package.Relay
{
    public interface IRelayMasterEventHandler
    {
        void OnDisconnected();
        void OnError(Exception relayEventError);
        void OnWarning(RelayException relayException, string s);
        void OnMessage(MessageFromClient messageFromClient);
        void OnClientConnected(ClientJoined clientJoined);
        void OnClientDisconnected(ClientDisconnected clientDisconnected);
        void OnConnected();
    }
}