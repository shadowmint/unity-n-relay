using System;
using N.Package.Relay.Infrastructure.Model;
using N.Package.WebSocket;

namespace N.Package.Relay
{
    public class RelayMasterOptions : WebSocketConnectionOptions
    {
        public TimeSpan TransactionTimeout = TimeSpan.FromSeconds(10);
        public MasterMetadata Metadata { get; set; }
    }
}