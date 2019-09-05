using System;
using System.Threading.Tasks;
using N.Package.Network;
using UnityEngine;

namespace Demo.Network
{
    [Serializable]
    public class DemoRpcMessage : NetworkCommand
    {
        public string message;
    }

    [Serializable]
    public class DemoRpcMessageResponse : NetworkCommand
    {
        public bool success;
        public string errorMessage;
    }

    public class DemoRpcMessageHandler : NetworkCommandHandler<DemoRpcMessage, DemoRpcMessageResponse>
    {
        public override Task<DemoRpcMessageResponse> ProcessRequestOnMaster(INetworkMaster master, DemoRpcMessage request, string clientId)
        {
            Debug.Log($"Master got message: {request.message} from {clientId}");
            return Task.FromResult(new DemoRpcMessageResponse()
            {
                success = true,
            });
        }

        public override Task<DemoRpcMessageResponse> ProcessRequestOnClient(INetworkClient client, DemoRpcMessage request)
        {
            Debug.Log($"Client got message: {request.message}");
            return Task.FromResult(new DemoRpcMessageResponse()
            {
                success = true,
            });
        }
    }
}