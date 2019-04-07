using System;
using N.Package.Relay.Infrastructure.Model;
using N.Package.WebSocket;

namespace N.Package.Relay
{
    [System.Serializable]
    public class RelayMasterOptions : WebSocketConnectionOptions
    {
        public TimeSpan transactionTimeout = TimeSpan.FromSeconds(10);

        public MasterMetadata metadata;

        public RelayAuthOptions auth;
    }

    [System.Serializable]
    public class RelayAuthOptions
    {
        public long sessionLength;

        public string authKey;

        public string authSecret;
    }
}