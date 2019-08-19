using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using N.Package.Promises;
using N.Package.Relay.Infrastructure;
using N.Package.Relay.Infrastructure.Model;
using N.Package.Relay.Infrastructure.TransactionManager;
using UnityEngine;

namespace N.Package.Network
{
    public class NetworkTransactionManager : IDisposable
    {
        private readonly Dictionary<string, INetworkTransactionDeferred> _pending = new Dictionary<string, INetworkTransactionDeferred>();

        private bool _active;

        /// <summary>
        /// Notice that the transaction manager can only respond to transactions which are pending.
        /// So in order for a request to be resolved, you must spool it using WaitFor *before* running
        /// the task that will generate a resolved transaction.
        ///
        /// Otherwise, you may end up in some situations waiting forever, because the result has been
        /// received before Register() has even been called; the transaction manager does *not* buffer
        /// unknown transactions, it just discards them. 
        /// </summary>
        public void Register<TRequest, TResult>(NetworkTransactionDeferred<TRequest, TResult> transaction)
            where TRequest : NetworkCommand
            where TResult : NetworkCommand
        {
            lock (_pending)
            {
                _pending[transaction.TransactionId] = transaction;
            }
        }

        public void HandleNetworkCommandResponse(NetworkCommand result, string raw)
        {
            lock (_pending)
            {
                if (!_pending.ContainsKey(result.commandInternalId))
                {
                    Debug.LogWarning($"Unknown transaction {result.commandInternalId} discarded. Did you use WaitFor correctly?");
                    return;
                }

                var transaction = _pending[result.commandInternalId];
                _pending.Remove(transaction.TransactionId);
                transaction.Resolve(result, raw);
            }
        }

        private void ExpireOldTransactions()
        {
            lock (_pending)
            {
                _pending.ToList().ForEach(i =>
                {
                    if (!i.Value.Expired) return;
                    i.Value.Reject(new RelayException(RelayErrorCode.TransactionTimeout));
                    _pending.Remove(i.Key);
                });
            }
        }

        public void SetEventLoop(bool active)
        {
            if (_active == active) return;
            _active = active;

            if (_active)
            {
                AsyncWorker.Run(EventLoop);
            }
        }

        private IEnumerator EventLoop()
        {
            while (_active)
            {
                ExpireOldTransactions();
                yield return new WaitForSeconds(0.01f);
            }
        }

        public void Dispose()
        {
            SetEventLoop(false);
        }
    }
}