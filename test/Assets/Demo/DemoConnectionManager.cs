using System.Threading.Tasks;
using N.Package.Network;
using N.Package.Network.Infrastructure;
using UnityEngine;

namespace Demo
{
    public class DemoConnectionManager : NetworkConnectionManager
    {
        protected override Task Configure()
        {
            Register(NetworkCommandType.FromClient, new NetworkCommandHandler());
            Register(NetworkCommandType.FromMaster, new NetworkCommandHandler());
            Register(NetworkCommandType.FromAny, new NetworkCommandHandler());
            return Task.CompletedTask;
        }

        protected override Task<string> Remote
        {
            get
            {
                var config = Object.FindObjectOfType<DemoConfig>();
                return Task.FromResult(config.remoteUrl);
            }
        }

        public async Task Connect(INetworkMaster master)
        {
            throw new System.NotImplementedException();
        }

        public async Task Connect(INetworkClient client)
        {
            throw new System.NotImplementedException();
        }
    }
}