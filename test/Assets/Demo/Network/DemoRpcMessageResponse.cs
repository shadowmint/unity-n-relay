using System;

namespace Demo.Network
{
    [Serializable]
    public class DemoRpcMessageResponse
    {
        public bool success;
        public string errorMessage;
    }
}