using System;
using System.Threading.Tasks;
using Demo.Network;
using N.Package.Network;
using N.Package.Promises;
using N.Package.Relay;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Demo
{
    class DemoServiceClient : INetworkClient
    {
        private readonly string _clientId;
        private string _username;

        public DemoServiceClient(string clientId)
        {
            _clientId = clientId;
        }

        public Guid Identity { get; set; }
        
        public NetworkConnection NetworkConnection { get; set; }

        public Task<RelayClientOptions> ClientOptions
        {
            get
            {
                var config = Object.FindObjectOfType<DemoConfig>();
                var options = config.clientOptions.Clone();
                options.metadata.name = _clientId;
                return Task.FromResult(config.clientOptions);
            }
        }

        public async Task OnConnectedToMasterAsync()
        {
            Debug.Log($"Client: {_clientId}: OK! Setting username...");
            var response = await NetworkConnection.Execute<DemoRpcSetUsername, DemoRpcSetUsernameResponse>(new DemoRpcSetUsername()
            {
                requestedPlayerName = _clientId
            });
            if (!response.success)
            {
                var try2 = await NetworkConnection.Execute<DemoRpcSetUsername, DemoRpcSetUsernameResponse>(new DemoRpcSetUsername()
                {
                    requestedPlayerName = response.suggestedName
                });
                if (!try2.success)
                {
                    throw new Exception("Failed to connect with a username");
                }
                else
                {
                    _username = response.suggestedName;
                }
            }
            else
            {
                _username = _clientId;
            }
            Debug.Log($"Client: {_clientId}: Username set to: {_username}");
        }

        public Task OnDisconnectedFromMasterAsync(string masterDisconnectedReason)
        {
            return Task.CompletedTask;
        }
    }
}