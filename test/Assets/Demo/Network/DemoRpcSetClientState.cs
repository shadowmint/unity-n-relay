using System;
using System.Threading.Tasks;
using N.Package.Network;
using UnityEngine;

namespace Demo.Network
{
    public enum DemoRpcClientState
    {
        AllowInput,
        DontAllowInput
    }

    [Serializable]
    public class DemoRpcSetClientState : NetworkCommand
    {
        public DemoRpcClientState state;
    }

    [Serializable]
    public class DemoRpcSetClientStateResponse : NetworkCommand
    {
        public bool success;
        public string errorMessage;
    }

    public class DemoRpcSetClientStateHandler : NetworkCommandHandler<DemoRpcSetClientState, DemoRpcSetClientStateResponse>
    {
        public override Task<DemoRpcSetClientStateResponse> ProcessRequestOnMaster(INetworkMaster master, DemoRpcSetClientState request)
        {
            return Task.FromResult(new DemoRpcSetClientStateResponse()
            {
                success = false,
                errorMessage = "Not supported"
            });
        }

        public override Task<DemoRpcSetClientStateResponse> ProcessRequestOnClient(INetworkClient client, DemoRpcSetClientState request)
        {
            var c = client as DemoServiceClient;
            Debug.Log($"{c.Name}: Client set state to: {request.state}");
            return Task.FromResult(new DemoRpcSetClientStateResponse()
            {
                success = true,
            });
        }
    }
}