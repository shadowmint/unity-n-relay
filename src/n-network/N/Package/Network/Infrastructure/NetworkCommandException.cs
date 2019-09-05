using System;

namespace N.Package.Network.Infrastructure
{
    public class NetworkCommandException : Exception
    {
        public NetworkCommandException(NetworkCommandError error) : base(error.message)
        {
        }
    }
}