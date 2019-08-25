using System;
using System.Threading.Tasks;
using UnityEngine;

namespace N.Package.Network.Infrastructure
{
    class NetworkCommandBinding
    {
        private Func<object, NetworkCommand, Task<NetworkCommand>> _handleRequest;

        private Func<string, NetworkCommand> _deserializeRequest;

        public string CommandInternalType { get; private set; }

        public static NetworkCommandBinding From<TRequest, TResponse>(NetworkCommandType source, NetworkCommandHandler<TRequest, TResponse> handler)
            where TRequest : NetworkCommand where TResponse : NetworkCommand
        {
            switch (source)
            {
                // Request that are from client are handled as requests on the master.
                case NetworkCommandType.FromClient:
                    return new NetworkCommandBinding()
                    {
                        CommandInternalType = NetworkCommand.CommandTypeFor(typeof(TRequest)),
                        _deserializeRequest = JsonUtility.FromJson<TRequest>,
                        _handleRequest = async (net, command) => await handler.ProcessRequestOnMaster(net as INetworkMaster, command as TRequest),
                    };

                // Request that are from master are handled as requests on the client.
                case NetworkCommandType.FromMaster:
                    return new NetworkCommandBinding()
                    {
                        CommandInternalType = NetworkCommand.CommandTypeFor(typeof(TRequest)),
                        _deserializeRequest = JsonUtility.FromJson<TRequest>,
                        _handleRequest = async (net, command) => await handler.ProcessRequestOnClient(net as INetworkClient, command as TRequest)
                    };

                default:
                    throw new ArgumentOutOfRangeException(nameof(source), source, null);
            }
        }

        public NetworkCommand DeserializeRequest(string raw)
        {
            return _deserializeRequest(raw);
        }

        public Task<NetworkCommand> HandleRequest<T>(T networkService, NetworkCommand command)
        {
            return _handleRequest(networkService, command);
        }
    }
}