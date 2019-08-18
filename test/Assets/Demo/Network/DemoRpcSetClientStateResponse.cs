using System;

namespace Demo.Network
{
    [Serializable]
    public class DemoRpcSetClientStateResponse
    {
        public bool success;
        public string errorMessage;
    }
}