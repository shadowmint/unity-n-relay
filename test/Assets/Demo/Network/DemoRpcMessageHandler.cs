using System;
using System.Threading.Tasks;
using N.Package.Network;
using UnityEngine;

namespace Demo.Network
{
    public class DemoRpcMessageHandler : NetworkCommandHandler<DemoRpcMessageHandler.DemoRpcMessage, DemoRpcMessageHandler.DemoRpcMessageResponse>
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

        public override Task<DemoRpcMessageResponse> ProcessRequestOnMaster(INetworkMaster master, DemoRpcMessage request)
        {
            Debug.Log($"Master got message: {request.message}");
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