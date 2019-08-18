using System;

namespace Demo.Network
{
    [Serializable]
    public class DemoRpcSetUsernameResponse
    {
        public bool success;
        public string errorMessage;
        public string suggestedName;
    }
}