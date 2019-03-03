using System;
using N.Package.Relay.Infrastructure.Model;
using N.Package.WebSocket;

namespace N.Package.Relay
{
    public class RelayClientOptions : WebSocketConnectionOptions
    {
        public TimeSpan TransactionTimeout = TimeSpan.FromSeconds(10);
        
        public ClientMetadata Metadata { get; set; }

        /// <summary>
        /// The session to join
        /// </summary>
        public string SessionId { get; set; }
    }
}