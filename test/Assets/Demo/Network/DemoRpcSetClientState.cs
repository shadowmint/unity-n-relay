using System;
using N.Package.Network;

namespace Demo.Network
{
    [Serializable]
    public class DemoRpcSetClientState : NetworkCommand
    {
        public DemoRpcClientState state;
    }
}