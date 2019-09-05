using System;
using System.Threading.Tasks;
using N.Package.Relay.Infrastructure;
using UnityEngine;

namespace N.Package.Network.Infrastructure
{
    public interface INetworkTransactionDeferred
    {
        string TransactionId { get; }
        bool Expired { get; }
        void Resolve(NetworkCommand result, string raw);
        void Reject(RelayException relayException);
    }

    public class NetworkTransactionDeferred<TRequest, TResult> : INetworkTransactionDeferred
        where TRequest : NetworkCommand
        where TResult : NetworkCommand
    {
        private readonly TRequest _request;
        private readonly TaskCompletionSource<TResult> _deferred;

        public DateTimeOffset Expires { get; }

        public Task<TResult> Task => _deferred.Task;

        public string TransactionId => _request.commandInternalId;
        
        public bool Expired => Expires > DateTimeOffset.Now;

        public void Resolve(NetworkCommand result, string raw)
        {
            try
            {
                var typedResponse = JsonUtility.FromJson<TResult>(raw);
                _deferred.SetResult(typedResponse);
            }
            catch (Exception error)
            {
                _deferred.SetException(new Exception("Invalid response object; unable to cast as the required type", error));
            }
        }

        public void Reject(RelayException error)
        {
            _deferred.SetException(error);
        }

        public NetworkTransactionDeferred(TRequest request, DateTimeOffset expires)
        {
            _request = request;
            _deferred = new TaskCompletionSource<TResult>();
            Expires = expires;
        }
    }
}