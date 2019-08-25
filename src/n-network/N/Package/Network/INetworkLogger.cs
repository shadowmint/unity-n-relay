using System;
using N.Package.Relay.Infrastructure;

namespace N.Package.Network
{
    public interface INetworkLogger
    {
        void OnError(Exception error);
        void OnWarning(RelayException error, string message);
    }
}