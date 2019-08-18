using System.Threading.Tasks;
using Demo.Network;
using N.Package.Network;
using N.Package.Network.Infrastructure;
using N.Package.Relay;
using N.Package.Relay.Infrastructure.TransactionManager;
using UnityEngine;

namespace Demo
{
    public class DemoConnectionManager : NetworkConnectionManager
    {
        protected override Task Configure()
        {
            Register(NetworkCommandType.FromClient, new DemoRpcMessageHandler());
            //   Register(NetworkCommandType.FromMaster, new NetworkCommandHandler());
//            Register(NetworkCommandType.FromAny, new NetworkCommandHandler());
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
    }
}