using System;
using System.Threading.Tasks;
using N.Package.Relay.Events.Client;
using N.Package.Relay.Infrastructure.Model;

namespace N.Package.Relay.Infrastructure.TransactionManager
{
    public class RelayDeferredTransaction
    {
        private readonly TaskCompletionSource<bool> _source = new TaskCompletionSource<bool>();

        private bool _resolved;

        /// <summary>
        /// What the unique id of this transaction is
        /// </summary>
        public string TransactionId { get; }

        /// <summary>
        /// When this transaction started
        /// </summary>
        public DateTime TransactionStarted { get; }

        /// <summary>
        /// When does this transaction expire?
        /// </summary>
        public DateTime TransactionExpires { get; }

        /// <summary>
        /// Has this transaction expired?
        /// </summary>
        public bool Expired => TransactionExpires < DateTime.Now;

        /// <summary>
        /// The source for this transaction
        /// </summary>
        public Task Task => _source.Task;

        public RelayDeferredTransaction(TimeSpan timeout)
        {
            TransactionId = Guid.NewGuid().ToString();
            TransactionStarted = DateTime.Now;
            TransactionExpires = TransactionStarted + timeout;
        }

        public void Resolve(TransactionResult result)
        {
            if (_resolved) return;
            _resolved = true;

            if (result.success)
            {
                _source.SetResult(true);
            }
            else
            {
                _source.SetException(new RelayException(result.error));
            }
        }

        public void Reject(Exception error)
        {
            if (_resolved) return;
            _resolved = true;
            _source.SetException(error);
        }
    }
}