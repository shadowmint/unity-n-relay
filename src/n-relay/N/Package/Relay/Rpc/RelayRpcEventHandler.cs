using System;
using N.Package.Relay.Events.Master.In;
using N.Package.Relay.Standard;

namespace N.Package.Relay.Rpc
{
    public class RelayRpcEventHandler : IStandardRelayEvents
    {
        private readonly IStandardRelayEvents _parentEventHandler;

        public RelayRpcEventHandler(IStandardRelayEvents parentEventHandler)
        {
            _parentEventHandler = parentEventHandler;
        }

        public void FailureDuringNormalOperation(Exception err)
        {
            throw new NotImplementedException();
        }

        public void PlayerMessage(MessageFromClient messageFromClient)
        {
            throw new NotImplementedException();
        }

        public void PlayerJoin(ClientJoined clientJoined)
        {
            throw new NotImplementedException();
        }

        public void PlayerParted(ClientDisconnected clientDisconnected)
        {
            throw new NotImplementedException();
        }
    }
}