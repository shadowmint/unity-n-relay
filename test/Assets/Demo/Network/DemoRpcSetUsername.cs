using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using N.Package.Network;

namespace Demo.Network
{
    [Serializable]
    public class DemoRpcSetUsername : NetworkCommand
    {
        public string requestedPlayerName;
    }

    [Serializable]
    public class DemoRpcSetUsernameResponse : NetworkCommand
    {
        public bool success;
        public string errorMessage;
        public string suggestedName;
    }

    public class DemoRpcSetUsernameHandler : NetworkCommandHandler<DemoRpcSetUsername, DemoRpcSetUsernameResponse>
    {
        private static readonly List<string> Users = new List<string>();

        public override Task<DemoRpcSetUsernameResponse> ProcessRequestOnMaster(INetworkMaster master, DemoRpcSetUsername request)
        {
            lock (Users)
            {
                if (Users.Contains(request.requestedPlayerName))
                {
                    return Task.FromResult(new DemoRpcSetUsernameResponse()
                    {
                        success = false,
                        errorMessage = "Invalid name, already in use",
                        suggestedName = $"{request.requestedPlayerName}-{Guid.NewGuid()}"
                    });
                }
                else
                {
                    Users.Add(request.requestedPlayerName);
                    return Task.FromResult(new DemoRpcSetUsernameResponse()
                    {
                        success = true
                    });
                }
            }
        }

        public override Task<DemoRpcSetUsernameResponse> ProcessRequestOnClient(INetworkClient client, DemoRpcSetUsername request)
        {
            return Task.FromResult(new DemoRpcSetUsernameResponse()
            {
                success = false,
                errorMessage = "Not supported"
            });
        }
    }
}