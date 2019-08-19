using System;
using N.Package.Network;
using N.Package.Relay.Infrastructure;

public class NetworkDefaultLogger : INetworkLogger
{
    public void OnError(Exception error)
    {
        UnityEngine.Debug.LogException(error);
    }

    public void OnWarning(RelayException error, string message)
    {
        UnityEngine.Debug.LogWarning(message);
        UnityEngine.Debug.LogException(error);
    }
}