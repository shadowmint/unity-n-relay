using System;
using N.Package.Network;

namespace Demo.Network
{
    [Serializable]
    public class DemoRpcSetUsernameResponse : NetworkCommand
    {
        public bool success;
        public string errorMessage;
        public string suggestedName;
    }
}