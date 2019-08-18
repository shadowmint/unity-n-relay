using System;
using N.Package.Relay.Infrastructure.Model;
using N.Package.WebSocket;
using UnityEngine;

namespace N.Package.Relay
{
    [System.Serializable]
    public class RelayClientOptions : WebSocketConnectionOptions
    {
        public TimeSpan transactionTimeout = TimeSpan.FromSeconds(10);

        public ClientMetadata metadata;

        public RelayAuthOptions auth;
        
        /// <summary>
        /// The session to join
        /// </summary>
        public string sessionId;

        public RelayClientOptions Clone()
        {
            return JsonUtility.FromJson<RelayClientOptions>(JsonUtility.ToJson(this));
        }
    }
}