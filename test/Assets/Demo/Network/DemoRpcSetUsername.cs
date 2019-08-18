using System;
using N.Package.Network;

namespace Demo.Network
{
    [Serializable]
    public class DemoRpcSetUsername : NetworkCommand
    {
        public string requestedPlayerName;
    }
}