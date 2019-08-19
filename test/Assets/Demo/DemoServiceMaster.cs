using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Demo.Network;
using N.Package.Network;
using N.Package.Relay;
using N.Package.Relay.Events.Master.In;
using UnityEngine;
using NetworkConnection = N.Package.Network.NetworkConnection;
using Object = UnityEngine.Object;

namespace Demo
{
    class DemoServiceMaster : INetworkMaster
    {
        private readonly List<string> _clients = new List<string>();

        public Guid Identity { get; set; }

        public NetworkConnection NetworkConnection { get; set; }

        public Task<RelayMasterOptions> MasterOptions
        {
            get
            {
                var config = Object.FindObjectOfType<DemoConfig>();
                return Task.FromResult(config.masterOptions);
            }
        }

        public Task OnMasterConnectedAsync()
        {
            Debug.Log("Master ready");
            return Task.CompletedTask;
        }

        public Task OnMasterConnectionLostAsync()
        {
            Debug.Log("Master offline");
            return Task.CompletedTask;
        }

        public Task OnMasterLostClientConnectionAsync(ClientDisconnected message)
        {
            Debug.Log($"Disconnected: {message.client_id}");
            _clients.RemoveAll(i => i == message.client_id);
            return Task.CompletedTask;
        }

        public async Task OnMasterGotClientConnectionAsync(ClientJoined message)
        {
            Debug.Log($"Client connected: {message.client_id}");
            _clients.Add(message.client_id);
            await NetworkConnection.Execute<DemoRpcSetClientState, DemoRpcSetClientStateResponse>(new DemoRpcSetClientState()
            {
                state = DemoRpcClientState.AllowInput
            });
            await NetworkConnection.Execute<DemoRpcMessageHandler.DemoRpcMessage, DemoRpcMessageHandler.DemoRpcMessageResponse>(
                new DemoRpcMessageHandler.DemoRpcMessage()
                {
                    message = "Welcome to server!"
                });
        }
    }
}