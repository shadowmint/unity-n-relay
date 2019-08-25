using System;
using N.Package.Relay.Infrastructure.Model;
using N.Package.WebSocket;
using UnityEngine;

namespace N.Package.Relay
{
    [System.Serializable]
    public class RelayMasterOptions : WebSocketConnectionOptions
    {
        public TimeSpan transactionTimeout = TimeSpan.FromSeconds(10);

        public MasterMetadata metadata;

        public RelayAuthOptions auth;

        public RelayMasterOptions Clone()
        {
            return JsonUtility.FromJson<RelayMasterOptions>(JsonUtility.ToJson(this));
        }
    }
}