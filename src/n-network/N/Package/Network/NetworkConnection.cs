using System;
using System.Threading.Tasks;
using N.Package.Network.Infrastructure;
using N.Package.Relay;

namespace N.Package.Network
{
    public class NetworkConnection : IDisposable
    {
        private readonly NetworkConnectionManager _manager;
        private readonly RelayClient _clientService;
        private readonly RelayMaster _masterService;
        private readonly NetworkTransactionManager _transactionManager;

        public NetworkConnection(NetworkConnectionManager manager, RelayMaster masterService, NetworkEventHandler eventHandler)
        {
            _manager = manager;
            _masterService = masterService;
            _transactionManager = new NetworkTransactionManager();
            eventHandler.Connected(this);
        }

        public NetworkConnection(NetworkConnectionManager manager, RelayClient clientService, NetworkEventHandler eventHandler)
        {
            _manager = manager;
            _clientService = clientService;
            _transactionManager = new NetworkTransactionManager();
            eventHandler.Connected(this);
        }

        public INetworkPermissions Permissions => _manager.Permissions;

        public async Task<TResponse> Execute<TRequest, TResponse>(TRequest request)
            where TResponse : NetworkCommand
            where TRequest : NetworkCommand
        {
            GuardIsClient();
            request.Prepare(true);
            var deferred = new NetworkTransactionDeferred<TRequest, TResponse>(request, DateTimeOffset.Now + await _manager.NetworkCommandTimeout);
            _transactionManager.Register(deferred);
            await _clientService.Send(request);
            return await deferred.Task;
        }


        public async Task<TResponse> Execute<TRequest, TResponse>(TRequest request, string clientId)
            where TResponse : NetworkCommand
            where TRequest : NetworkCommand
        {
            GuardIsMaster();
            request.Prepare(true);
            var deferred = new NetworkTransactionDeferred<TRequest, TResponse>(request, DateTimeOffset.Now + await _manager.NetworkCommandTimeout);
            _transactionManager.Register(deferred);
            await _masterService.Send(clientId, request);
            return await deferred.Task;
        }

        public Task Send<T>(T message)
        {
            GuardIsClient();
            return _clientService.Send(message);
        }

        public Task Send<T>(T message, string clientId)
        {
            GuardIsMaster();
            return _masterService.Send(clientId, message);
        }

        public void ResolveTransaction(NetworkCommand command, string raw)
        {
            _transactionManager.HandleNetworkCommandResponse(command, raw);
        }

        public void Dispose()
        {
            _transactionManager.Dispose();
        }

        private void GuardIsClient()
        {
            if (_clientService == null && _masterService != null)
            {
                throw new NetworkException(NetworkException.NetworkExceptionType.InvalidClientIdOnClientRequest);
            }
        }

        private void GuardIsMaster()
        {
            if (_masterService == null && _clientService != null)
            {
                throw new NetworkException(NetworkException.NetworkExceptionType.MissingClientIdOnMasterRequest);
            }
        }
    }
}