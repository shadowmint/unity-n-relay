using System;
using N.Package.Relay.Infrastructure.Model;
using N.Package.WebSocket;

namespace N.Package.Relay
{
    [System.Serializable]
    public class RelayClientOptions : WebSocketConnectionOptions
    {
        public TimeSpan transactionTimeout = TimeSpan.FromSeconds(10);

        public ClientMetadata metadata;

        /// <summary>
        /// The session to join
        /// </summary>
        public string sessionId;
    }
}