using System;
using N.Package.Network;

namespace Demo.Network
{
    [Serializable]
    public class DemoRpcSetClientStateResponse : NetworkCommand
    {
        public bool success;
        public string errorMessage;
    }
}