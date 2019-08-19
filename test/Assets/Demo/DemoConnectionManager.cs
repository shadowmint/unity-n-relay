using System;
using System.Threading.Tasks;
using Demo.Network;
using N.Package.Network;
using N.Package.Network.Infrastructure;
using N.Package.Relay;
using N.Package.Relay.Infrastructure.TransactionManager;
using Object = UnityEngine.Object;

namespace Demo
{
    public class DemoConnectionManager : NetworkConnectionManager
    {
        private readonly TimeSpan _networkCommandTimeout = TimeSpan.FromSeconds(30);

        public override INetworkLogger Logger { get; } = new NetworkDefaultLogger();

        public override Task<TimeSpan> NetworkCommandTimeout => Task.FromResult(_networkCommandTimeout);

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