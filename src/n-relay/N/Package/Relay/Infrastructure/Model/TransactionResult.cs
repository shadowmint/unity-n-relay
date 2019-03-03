using System.Diagnostics.CodeAnalysis;
using N.Package.Relay.Infrastructure.Events;

namespace N.Package.Relay.Infrastructure.Model
{
    [System.Serializable]
    public class TransactionResult : RelayIncomingEvent, IRelayTransactionEvent
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public string transaction_id;

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public bool success;

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public ExternalError error;

        public string TransactionId
        {
            get { return transaction_id; }
            set { transaction_id = value; }
        }
    }
}