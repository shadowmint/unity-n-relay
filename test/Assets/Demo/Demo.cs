using System.Threading.Tasks;
using Demo.Network;
using N.Package.Network;
using N.Package.Promises;
using UnityEngine;

namespace Demo
{
    public class Demo : MonoBehaviour
    {
        private DemoConnectionManager _connectionMaster;

        public void Start()
        {
            StartAsync().Promise().Dispatch();
        }

        private async Task StartAsync()
        {
            _connectionMaster = new DemoConnectionManager();
            await _connectionMaster.Connect(new DemoServiceMaster());
            await _connectionMaster.Connect(new DemoServiceClient("Client 1"));
            await _connectionMaster.Connect(new DemoServiceClient("Client 2"));
        }
    }
}