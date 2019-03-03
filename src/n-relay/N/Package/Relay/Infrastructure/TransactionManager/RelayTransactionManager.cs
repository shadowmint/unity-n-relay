using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using N.Package.Promises;
using N.Package.Relay.Events.Client;
using N.Package.Relay.Infrastructure.Model;
using UnityEngine;

namespace N.Package.Relay.Infrastructure.TransactionManager
{
    public class RelayTransactionManager
    {
        private readonly Dictionary<string, RelayDeferredTransaction> _pending = new Dictionary<string, RelayDeferredTransaction>();

        private bool _active;

        public Task WaitFor(RelayDeferredTransaction transaction)
        {
            lock (_pending)
            {
                _pending[transaction.TransactionId] = transaction;
                return transaction.Task;
            }
        }

        public void HandleTransactionResult(TransactionResult result)
        {
            lock (_pending)
            {
                if (!_pending.ContainsKey(result.transaction_id)) return;
                var transaction = _pending[result.transaction_id];
                _pending.Remove(transaction.TransactionId);
                transaction.Resolve(result);
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
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}