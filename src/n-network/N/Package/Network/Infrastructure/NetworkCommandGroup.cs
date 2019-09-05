using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace N.Package.Network.Infrastructure
{
    public class NetworkCommandGroup
    {
        private List<NetworkCommandBinding> _fromMaster = new List<NetworkCommandBinding>();

        private List<NetworkCommandBinding> _fromClient = new List<NetworkCommandBinding>();

        public NetworkCommandGroup Clone()
        {
            return new NetworkCommandGroup()
            {
                _fromClient = _fromClient.ToList(),
                _fromMaster = _fromMaster.ToList()
            };
        }

        public void Register<TRequest, TResponse>(NetworkCommandType source, NetworkCommandHandler<TRequest, TResponse> handler)
            where TResponse : NetworkCommand
            where TRequest : NetworkCommand
        {
            switch (source)
            {
                case NetworkCommandType.FromClient:
                    _fromClient.Add(NetworkCommandBinding.From(source, handler));
                    break;
                case NetworkCommandType.FromMaster:
                    _fromMaster.Add(NetworkCommandBinding.From(source, handler));
                    break;
                case NetworkCommandType.FromAny:
                    _fromClient.Add(NetworkCommandBinding.From(NetworkCommandType.FromClient, handler));
                    _fromMaster.Add(NetworkCommandBinding.From(NetworkCommandType.FromMaster, handler));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(source), source, null);
            }
        }

        public async Task<NetworkCommand> ProcessIncomingMessage(NetworkConnection connection, NetworkCommand command, string raw, INetworkMaster master, string fromClientId)
        {
            // This is correct; we handle requests from clients on the master.
            var binding = FindBindingFor(command, _fromClient);
            if (binding == null)
            {
                throw new NetworkException(NetworkException.NetworkExceptionType.UnsupportedCommandType, command.commandInternalType);
            }

            try
            {
                var fullRequest = binding.DeserializeRequest(raw);
                await VerifyPermission(connection, fullRequest, master, fromClientId);
                return await binding.HandleRequest(master, fullRequest, fromClientId);
            }
            catch (Exception error)
            {
                throw new NetworkException(NetworkException.NetworkExceptionType.CommandFailed, error);
            }
        }

        public async Task<NetworkCommand> ProcessIncomingMessage(NetworkConnection connection, NetworkCommand command, string raw, INetworkClient client)
        {
            // This is correct; we handle requests from master on the clients.
            var binding = FindBindingFor(command, _fromMaster);
            if (binding == null)
            {
                throw new NetworkException(NetworkException.NetworkExceptionType.UnsupportedCommandType, command.commandInternalType);
            }

            try
            {
                var fullRequest = binding.DeserializeRequest(raw);
                await VerifyPermission(connection, fullRequest, client);
                return await binding.HandleRequest(client, fullRequest);
            }
            catch (Exception error)
            {
                throw new NetworkException(NetworkException.NetworkExceptionType.CommandFailed, error);
            }
        }

        private NetworkCommandBinding FindBindingFor(NetworkCommand command, List<NetworkCommandBinding> bindings)
        {
            return bindings.FirstOrDefault(i => i.CommandInternalType == command.commandInternalType);
        }

        private async Task VerifyPermission(NetworkConnection connection, NetworkCommand command, INetworkMaster master, string fromClientId)
        {
            if (!await connection.Permissions.IsPermittedOnMaster(command, master, fromClientId))
            {
                throw new NetworkException(NetworkException.NetworkExceptionType.NotPermitted);
            }
        }

        private async Task VerifyPermission(NetworkConnection connection, NetworkCommand command, INetworkClient client)
        {
            if (!await connection.Permissions.IsPermittedOnClient(command, client))
            {
                throw new NetworkException(NetworkException.NetworkExceptionType.NotPermitted);
            }
        }
    }
}