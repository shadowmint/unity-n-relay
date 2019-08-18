using System;
using N.Package.Relay.Events.Master.In;

namespace N.Package.Relay.Standard
{
    public interface IStandardRelayEvents
    {
        void FailureDuringNormalOperation(Exception err);
        void PlayerMessage(MessageFromClient messageFromClient);
        void PlayerJoin(ClientJoined clientJoined);
        void PlayerParted(ClientDisconnected clientDisconnected);
    }
}