using System;
using N.Package.Relay.Events.Client;
using N.Package.Relay.Events.Client.In;
using N.Package.Relay.Infrastructure;

namespace N.Package.Relay
{
    public interface IRelayClientEventHandler
    {
        void OnError(Exception error);
        void OnConnected();
        void OnDisconnected();
        void OnWarning(Exception relayException, string message);
        void OnMessage(MessageToClient message);
        void OnMasterDisconnected(MasterDisconnected masterDisconnected);
    }
}