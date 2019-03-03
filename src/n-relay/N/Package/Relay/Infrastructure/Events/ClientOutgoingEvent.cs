using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace N.Package.Relay.Infrastructure.Events
{
    public abstract class ClientOutgoingEvent : RelayOutgoingEvent, IRelayTransactionEvent
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public string transaction_id;

        public string TransactionId
        {
            get { return transaction_id; }
            set { transaction_id = value; }
        }
    }
}